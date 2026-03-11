using RestaurantSystem.Infrastructure.Persistence;
using RestaurantSystem.WebAPI.Extensions;
using RestaurantSystem.Infrastructure.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configuration sources: appsettings, user secrets (dev), env vars, and command line.
builder.AddAppConfiguration(args);

// Kestrel HTTPS settings (TLS 1.2/1.3 only).
builder.AddKestrelSecurity();

// Security headers and forwarding settings.
builder.Services.AddSecurityHeaders();
builder.Services.ConfigureForwardedHeaders(builder.Configuration);

// Database and EF Core: ConnectionStrings:DefaultConnection, EfCore:*.
builder.Services.AddDatabase(builder.Configuration);

// API versioning + Swagger/OpenAPI.
builder.Services.AddApiVersioningAndSwagger();

// Controllers, ProblemDetails, MediatR, validators, and app services.
builder.Services.AddWebApiServices(builder.Environment);

// JWT auth: JwtSettings:*.
builder.Services.AddAuthServices(builder.Configuration);

// Health checks + CORS (uses DefaultConnection for DB check).
builder.Services.AddHealthChecksAndCors(builder.Configuration);

// rate limiting policies and middleware
builder.Services.AddApiRateLimiting();

var app = builder.Build();

// SEED DATA (Development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await initializer.SeedAsync();
}

app.UseWebApiPipeline();

app.Run();
