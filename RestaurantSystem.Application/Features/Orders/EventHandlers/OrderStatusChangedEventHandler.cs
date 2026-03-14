using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public class OrderStatusChangedEventHandler : INotificationHandler<OrderStatusChangedEvent>
{
    private readonly ILogger<OrderStatusChangedEventHandler> _logger;
    private readonly ISignalRService _signalR;
    private readonly IApplicationDbContext _context;

    public OrderStatusChangedEventHandler(
        ILogger<OrderStatusChangedEventHandler> logger,
        ISignalRService signalR,
        IApplicationDbContext context)
    {
        _logger = logger;
        _signalR = signalR;
        _context = context;
    }

    public async Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Order status changed: {OrderId} {FromStatus} -> {ToStatus}",
            notification.OrderId, notification.FromStatus, notification.ToStatus);

        var now = DateTime.UtcNow;
        await _signalR.NotifyOrderStatusChanged(
            notification.OrderId,
            notification.FromStatus.ToString(),
            notification.ToStatus.ToString(),
            now);

        int? userId = null;
        var order = await _context.Orders
            .AsNoTracking()
            .Select(o => new { o.Id, o.CustomerId })
            .FirstOrDefaultAsync(o => o.Id == notification.OrderId, cancellationToken);

        if (order?.CustomerId != null)
        {
            userId = await _context.Customers
                .AsNoTracking()
                .Where(c => c.Id == order.CustomerId)
                .Select(c => (int?)c.UserId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (notification.ToStatus == OrderStatus.Ready)
        {
            await _signalR.NotifyOrderReady(notification.OrderId, now, userId);
        }
        else if (notification.ToStatus == OrderStatus.Cancelled)
        {
            await _signalR.NotifyOrderCancelled(notification.OrderId, now, userId);
        }

        await _signalR.NotifyDashboardStatsUpdated(now);
    }
}
