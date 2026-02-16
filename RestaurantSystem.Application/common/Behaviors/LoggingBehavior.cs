using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that logs all requests
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId;
        var userEmail = _currentUserService.Email ?? "Anonymous";

        _logger.LogInformation(
            "Restaurant System Request: {Name} by {UserEmail} (ID: {UserId}) {@Request}",
            requestName, userEmail, userId, request);

        try
        {
            var response = await next();

            _logger.LogInformation(
                "Restaurant System Request Completed: {Name}",
                requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Restaurant System Request Failed: {Name} by {UserEmail} (ID: {UserId})",
                requestName, userEmail, userId);
            throw;
        }
    }
}