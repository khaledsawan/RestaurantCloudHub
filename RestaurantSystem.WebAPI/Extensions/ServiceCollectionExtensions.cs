using System.Security.Authentication;
using System.Text;
using System.Net;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RestaurantSystem.Application.Common.Behaviors;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Auth.Commands.Register;
using RestaurantSystem.Infrastructure.Identity;
using RestaurantSystem.Infrastructure.Options;
using RestaurantSystem.Infrastructure.Persistence;
using RestaurantSystem.Infrastructure.Persistence.Interceptors;
using RestaurantSystem.Infrastructure.Services;
using RestaurantSystem.WebAPI.Conventions;
using RestaurantSystem.WebAPI.Services;

namespace RestaurantSystem.WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddAppConfiguration(this WebApplicationBuilder builder, string[] args)
    {
        // Load configuration from appsettings, secrets (dev), env vars, then command line (highest priority).
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);
        }

        builder.Configuration.AddEnvironmentVariables();

        if (args is { Length: > 0 })
        {
            builder.Configuration.AddCommandLine(args);
        }

        return builder;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // ConnectionStrings:DefaultConnection, EfCore:EnableSensitiveDataLogging, EfCore:EnableDetailedErrors
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Required configuration 'ConnectionStrings:DefaultConnection' is missing. " +
                                   "Set it via user secrets for Development or environment variable 'ConnectionStrings__DefaultConnection'.");

        var enableSensitiveDataLogging = configuration.GetValue<bool?>("EfCore:EnableSensitiveDataLogging") ?? false;
        var enableDetailedErrors = configuration.GetValue<bool?>("EfCore:EnableDetailedErrors") ?? false;

        services.Configure<AuditOptions>(configuration.GetSection("Audit"));
        services.Configure<OrderSettings>(configuration.GetSection(OrderSettings.SectionName));
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddScoped<AuditLogInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString);
            options.EnableSensitiveDataLogging(enableSensitiveDataLogging);
            options.EnableDetailedErrors(enableDetailedErrors);
            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<SoftDeleteInterceptor>(),
                sp.GetRequiredService<AuditLogInterceptor>());
        });

        return services;
    }

    public static IServiceCollection AddApiVersioningAndSwagger(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant API v1", Version = "v1" });
            options.SwaggerDoc("v2", new OpenApiInfo { Title = "Restaurant API v2", Version = "v2" });
            options.SwaggerDoc("health", new OpenApiInfo { Title = "Health Checks", Version = "v1" });

            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (docName == "health")
                {
                    return apiDesc.GroupName == "health";
                }
                return apiDesc.GroupName == docName;
            });

            const string securitySchemeName = "Bearer";

            options.AddSecurityDefinition(securitySchemeName, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            // Swashbuckle 10: use delegate and built-in reference type.
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(securitySchemeName, document)] = new List<string>()
            });
        });

        return services;
    }

    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddControllers(options =>
            {
                options.Conventions.Add(new SuccessResponseTypeConvention());
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest));
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status401Unauthorized));
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status403Forbidden));
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status404NotFound));
                options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase));
            });

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

                if (!environment.IsDevelopment())
                {
                    context.ProblemDetails.Extensions.Remove("exception");
                }
            };
        });

        services.AddSignalR();

        services.AddHttpContextAccessor();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterCommand>());
        services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IOrderSettings, OrderSettingsService>();
        services.AddScoped<ISignalRService, SignalRService>();
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        services.AddHostedService<QueuedHostedService>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();

        return services;
    }

    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        // JwtSettings:SecretKey, JwtSettings:Issuer, JwtSettings:Audience
        var jwtSecret = configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey is required");
        if (jwtSecret.Length < 32)
        {
            throw new InvalidOperationException("JwtSettings:SecretKey must be at least 32 characters.");
        }
        var jwtIssuer = configuration["JwtSettings:Issuer"] ?? "RestaurantSystem";
        var jwtAudience = configuration["JwtSettings:Audience"] ?? "RestaurantSystemAPI";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }

    public static IServiceCollection AddHealthChecksAndCors(this IServiceCollection services, IConfiguration configuration)
    {
        // ConnectionStrings:DefaultConnection used by health checks.
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Required configuration 'ConnectionStrings:DefaultConnection' is missing. " +
                                   "Set it via user secrets for Development or environment variable 'ConnectionStrings__DefaultConnection'.");

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy =>
                {
                    var corsBuilder = allowedOrigins.Length > 0
                        ? policy.WithOrigins(allowedOrigins)
                        : policy.WithOrigins("http://localhost:4200");

                    corsBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddNpgSql(connectionString, name: "postgresql", tags: ["ready"]);

        return services;
    }

    public static IServiceCollection AddSecurityHeaders(this IServiceCollection services)
    {
        services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            options.HttpsPort = 443;
        });

        services.AddHsts(options =>
        {
            options.MaxAge = TimeSpan.FromDays(365);
            options.IncludeSubDomains = true;
            options.Preload = true;
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        return services;
    }

    public static IServiceCollection ConfigureForwardedHeaders(this IServiceCollection services, IConfiguration configuration)
    {
        var proxyIps = configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? Array.Empty<string>();
        var networkCidrs = configuration.GetSection("ForwardedHeaders:KnownIPNetworks").Get<string[]>() ?? Array.Empty<string>();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            if (proxyIps.Length == 0 && networkCidrs.Length == 0)
            {
                return;
            }

            options.KnownProxies.Clear();
            options.KnownIPNetworks.Clear();

            foreach (var ip in proxyIps)
            {
                if (IPAddress.TryParse(ip, out var address))
                {
                    options.KnownProxies.Add(address);
                }
            }

            foreach (var cidr in networkCidrs)
            {
                var parts = cidr.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length != 2)
                {
                    continue;
                }

                if (IPAddress.TryParse(parts[0], out var networkAddress) &&
                    int.TryParse(parts[1], out var prefixLength))
                {
                    options.KnownIPNetworks.Add(new System.Net.IPNetwork(networkAddress, prefixLength));
                }
            }
        });

        return services;
    }

    public static WebApplicationBuilder AddKestrelSecurity(this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            });
        });

        return builder;
    }
}
