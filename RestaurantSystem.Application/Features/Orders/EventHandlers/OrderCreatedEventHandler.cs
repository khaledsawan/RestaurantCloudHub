using MediatR;
using Microsoft.Extensions.Logging;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order created: {OrderId} {OrderNumber}", notification.OrderId, notification.OrderNumber);
        return Task.CompletedTask;
    }
}
