using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand : IRequest<Result>
{
    public int OrderId { get; init; }
    public string? Reason { get; init; }
}
