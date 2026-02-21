using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Orders.Commands.AssignDriver;

public record AssignDriverCommand : IRequest<Result>
{
    public int OrderId { get; init; }
    public int StaffId { get; init; }
}
