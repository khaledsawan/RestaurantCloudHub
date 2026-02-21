using MediatR;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public record OrderCompletedEvent(int OrderId, string OrderNumber) : INotification;
