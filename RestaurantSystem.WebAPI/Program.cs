using System.Security.Authentication;
using System.Text;
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
using RestaurantSystem.Infrastructure.Persistence;
using RestaurantSystem.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 443;
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
    });
});

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// DATABASE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException(
                           "Required configuration 'ConnectionStrings:DefaultConnection' is missing. " +
                           "Set it via user secrets for Development or environment variable 'ConnectionStrings__DefaultConnection'.");

var enableSensitiveDataLogging = builder.Configuration.GetValue<bool?>("EfCore:EnableSensitiveDataLogging") ?? false;
var enableDetailedErrors = builder.Configuration.GetValue<bool?>("EfCore:EnableDetailedErrors") ?? false;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging(enableSensitiveDataLogging);
    options.EnableDetailedErrors(enableDetailedErrors);
});

// VERSIONING
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// CONTROLLERS (API)
builder.Services.AddControllers();

// HTTP CONTEXT ACCESSOR (for CurrentUserService)
builder.Services.AddHttpContextAccessor();

// MEDIATR + VALIDATION
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterCommand>());
builder.Services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// AUTH SERVICES
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDateTime, DateTimeService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ApplicationDbContextInitializer>();

// JWT AUTH
var jwtSecret = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is required");
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "RestaurantSystem";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "RestaurantSystemAPI";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

// SWAGGER / OPENAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

    // This is the new API for Swashbuckle 10 — use a delegate and the built-in reference type
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(securitySchemeName, document)] = new List<string>()
    });
});

// HEALTHCHECKS
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddNpgSql(connectionString, name: "postgresql", tags: ["ready"]);

var app = builder.Build();

// SEED DATA
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await initializer.SeedAsync();
}

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "API v2");
        c.SwaggerEndpoint("/swagger/health/swagger.json", "Health Checks");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
