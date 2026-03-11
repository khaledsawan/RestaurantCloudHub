using Infrastructure.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace RestaurantSystem.Infrastructure.RateLimiting;

public static class RateLimiterOptionsSetup
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            ConfigureLoginLimiter(options);
            ConfigureRegisterLimiter(options);
            ConfigureOtpLimiter(options);
            ConfigureGlobalLimiter(options);
        });

        return services;
    }

    private static void ConfigureLoginLimiter(RateLimiterOptions options)
    {
        options.AddPolicy(RateLimiterPolicies.Login, httpContext =>
            RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 5,
                    QueueLimit = 0
                }));
    }

    private static void ConfigureRegisterLimiter(RateLimiterOptions options)
    {
        options.AddPolicy(RateLimiterPolicies.Register, httpContext =>
            RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 10,
                    TokensPerPeriod = 5,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    AutoReplenishment = true,
                    QueueLimit = 0
                }));
    }

    private static void ConfigureOtpLimiter(RateLimiterOptions options)
    {
        options.AddPolicy(RateLimiterPolicies.SendOtp, httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 3,
                    Window = TimeSpan.FromMinutes(5),
                    QueueLimit = 0
                }));
    }

    private static void ConfigureGlobalLimiter(RateLimiterOptions options)
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext => RateLimitPartition.GetTokenBucketLimiter(
               RateLimiterPolicies.Global,
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 1000,
                    TokensPerPeriod = 500,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                    AutoReplenishment = true,
                    QueueLimit = 0
                }));
    }
}