using MediatR;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public record OrderCreatedEvent(int OrderId, string OrderNumber, int CustomerId) : INotification;
