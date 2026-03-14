using MediatR;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public record DeliveryAssignedEvent(int OrderId, int DriverId) : INotification;
