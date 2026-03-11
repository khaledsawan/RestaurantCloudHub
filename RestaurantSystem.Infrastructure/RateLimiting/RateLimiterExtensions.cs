using Microsoft.AspNetCore.Builder;

namespace Infrastructure.RateLimiting;

public static class RateLimiterExtensions
{
    public static IApplicationBuilder UseApiRateLimiting(this IApplicationBuilder app)
    {
        app.UseRateLimiter();
        return app;
    }
}