using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that monitors performance and logs slow requests
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        // Log if request takes longer than 500ms
        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId;
            var userEmail = _currentUserService.Email ?? "Anonymous";

            _logger.LogWarning(
                "Restaurant System Long Running Request: {Name} ({ElapsedMilliseconds} ms) by {UserEmail} (ID: {UserId}) {@Request}",
                requestName, elapsedMilliseconds, userEmail, userId, request);
        }

        return response;
    }
}