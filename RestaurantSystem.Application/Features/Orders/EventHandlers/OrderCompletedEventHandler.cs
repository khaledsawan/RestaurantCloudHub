using MediatR;
using Microsoft.Extensions.Logging;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public class OrderCompletedEventHandler : INotificationHandler<OrderCompletedEvent>
{
    private readonly ILogger<OrderCompletedEventHandler> _logger;

    public OrderCompletedEventHandler(ILogger<OrderCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order completed: {OrderId} {OrderNumber}", notification.OrderId, notification.OrderNumber);
        return Task.CompletedTask;
    }
}
