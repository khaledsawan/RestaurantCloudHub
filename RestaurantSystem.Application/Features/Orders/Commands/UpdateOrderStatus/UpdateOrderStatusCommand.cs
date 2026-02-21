using MediatR;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.Commands.UpdateOrderStatus;

public record UpdateOrderStatusCommand : IRequest<Result>
{
    public int OrderId { get; init; }
    public OrderStatus Status { get; init; }
    public string? Notes { get; init; }
}
