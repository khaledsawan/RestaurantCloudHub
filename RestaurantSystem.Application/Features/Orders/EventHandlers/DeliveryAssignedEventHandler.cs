using MediatR;
using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public class DeliveryAssignedEventHandler : INotificationHandler<DeliveryAssignedEvent>
{
    private readonly ISignalRService _signalR;

    public DeliveryAssignedEventHandler(ISignalRService signalR)
    {
        _signalR = signalR;
    }

    public Task Handle(DeliveryAssignedEvent notification, CancellationToken cancellationToken)
    {
        return _signalR.NotifyDeliveryAssigned(notification.OrderId, notification.DriverId, DateTime.UtcNow);
    }
}
