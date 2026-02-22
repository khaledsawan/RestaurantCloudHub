using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Reservations.Commands.CancelReservation;

public record CancelReservationCommand : IRequest<Result>
{
    public int ReservationId { get; init; }
    public string? Reason { get; init; }
}
