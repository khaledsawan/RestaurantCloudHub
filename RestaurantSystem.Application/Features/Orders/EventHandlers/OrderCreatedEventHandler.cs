using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    private readonly ISignalRService _signalR;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger, ISignalRService signalR)
    {
        _logger = logger;
        _signalR = signalR;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order created: {OrderId} {OrderNumber}", notification.OrderId, notification.OrderNumber);

        var now = DateTime.UtcNow;
        await _signalR.NotifyOrderCreated(notification.OrderId, notification.OrderNumber, now);
        await _signalR.NotifyNewOrderInKitchen(notification.OrderId, now);
        await _signalR.NotifyDashboardStatsUpdated(now);
    }
}
