using MediatR;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.EventHandlers;

public record OrderStatusChangedEvent(int OrderId, OrderStatus FromStatus, OrderStatus ToStatus) : INotification;
