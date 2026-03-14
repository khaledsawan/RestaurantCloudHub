using Microsoft.AspNetCore.SignalR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Infrastructure.Hubs;

namespace RestaurantSystem.Infrastructure.Services;

public class SignalRService : ISignalRService
{
    private readonly IHubContext<OrderHub> _orderHub;
    private readonly IHubContext<KitchenHub> _kitchenHub;
    private readonly IHubContext<DashboardHub> _dashboardHub;
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IHubContext<DeliveryHub> _deliveryHub;

    public SignalRService(
        IHubContext<OrderHub> orderHub,
        IHubContext<KitchenHub> kitchenHub,
        IHubContext<DashboardHub> dashboardHub,
        IHubContext<NotificationHub> notificationHub,
        IHubContext<DeliveryHub> deliveryHub)
    {
        _orderHub = orderHub;
        _kitchenHub = kitchenHub;
        _dashboardHub = dashboardHub;
        _notificationHub = notificationHub;
        _deliveryHub = deliveryHub;
    }

    public Task NotifyOrderCreated(int orderId, string orderNumber, DateTime createdAt)
    {
        return _orderHub.Clients.Groups("Admins", "Dashboard").SendAsync("OrderCreated", new
        {
            OrderId = orderId,
            OrderNumber = orderNumber,
            CreatedAt = createdAt
        });
    }

    public Task NotifyOrderStatusChanged(int orderId, string fromStatus, string toStatus, DateTime changedAt)
    {
        return _orderHub.Clients.Groups("Admins", "Dashboard").SendAsync("OrderStatusChanged", new
        {
            OrderId = orderId,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ChangedAt = changedAt
        });
    }

    public async Task NotifyOrderReady(int orderId, DateTime readyAt, int? userId = null)
    {
        await _orderHub.Clients.Groups("Admins", "Dashboard").SendAsync("OrderReady", new
        {
            OrderId = orderId,
            ReadyAt = readyAt
        });

        if (userId.HasValue)
        {
            await _notificationHub.Clients.User(userId.Value.ToString()).SendAsync("NotificationReceived", new
            {
                UserId = userId.Value,
                Message = $"Order #{orderId} is ready.",
                Type = "order_ready",
                Timestamp = readyAt
            });
        }
    }

    public async Task NotifyOrderCancelled(int orderId, DateTime cancelledAt, int? userId = null)
    {
        await _orderHub.Clients.Groups("Admins", "Dashboard").SendAsync("OrderCancelled", new
        {
            OrderId = orderId,
            CancelledAt = cancelledAt
        });

        if (userId.HasValue)
        {
            await _notificationHub.Clients.User(userId.Value.ToString()).SendAsync("NotificationReceived", new
            {
                UserId = userId.Value,
                Message = $"Order #{orderId} was cancelled.",
                Type = "order_cancelled",
                Timestamp = cancelledAt
            });
        }
    }

    public Task NotifyDashboardStatsUpdated(DateTime timestamp)
    {
        return _dashboardHub.Clients.Group("Dashboard").SendAsync("DashboardStatsUpdated", new
        {
            Timestamp = timestamp
        });
    }

    public Task NotifyNotificationReceived(int userId, string message, string type, DateTime timestamp)
    {
        return _notificationHub.Clients.User(userId.ToString()).SendAsync("NotificationReceived", new
        {
            UserId = userId,
            Message = message,
            Type = type,
            Timestamp = timestamp
        });
    }

    public Task NotifyNewOrderInKitchen(int orderId, DateTime createdAt)
    {
        return _kitchenHub.Clients.Group("Kitchen").SendAsync("NewOrderInKitchen", new
        {
            OrderId = orderId,
            CreatedAt = createdAt
        });
    }

    public Task NotifyDeliveryAssigned(int orderId, int driverId, DateTime assignedAt)
    {
        return _deliveryHub.Clients.Group("Delivery").SendAsync("DeliveryAssigned", new
        {
            OrderId = orderId,
            DriverId = driverId,
            AssignedAt = assignedAt
        });
    }
}
