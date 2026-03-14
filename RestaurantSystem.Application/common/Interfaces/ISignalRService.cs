namespace RestaurantSystem.Application.Common.Interfaces;

public interface ISignalRService
{
    Task NotifyOrderCreated(int orderId, string orderNumber, DateTime createdAt);
    Task NotifyOrderStatusChanged(int orderId, string fromStatus, string toStatus, DateTime changedAt);
    Task NotifyOrderReady(int orderId, DateTime readyAt, int? userId = null);
    Task NotifyOrderCancelled(int orderId, DateTime cancelledAt, int? userId = null);
    Task NotifyDashboardStatsUpdated(DateTime timestamp);
    Task NotifyNotificationReceived(int userId, string message, string type, DateTime timestamp);
    Task NotifyNewOrderInKitchen(int orderId, DateTime createdAt);
    Task NotifyDeliveryAssigned(int orderId, int driverId, DateTime assignedAt);
}
