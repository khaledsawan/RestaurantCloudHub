using MediatR;
using Microsoft.Extensions.Logging;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public class OrderStatusChangedEventHandler : INotificationHandler<OrderStatusChangedEvent>
{
    private readonly ILogger<OrderStatusChangedEventHandler> _logger;

    public OrderStatusChangedEventHandler(ILogger<OrderStatusChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order status changed: {OrderId} {FromStatus} -> {ToStatus}",
            notification.OrderId, notification.FromStatus, notification.ToStatus);
        return Task.CompletedTask;
    }
}
