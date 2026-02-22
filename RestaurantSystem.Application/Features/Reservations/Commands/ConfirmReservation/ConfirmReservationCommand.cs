using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Reservations.Commands.ConfirmReservation;

public record ConfirmReservationCommand : IRequest<Result>
{
    public int ReservationId { get; init; }
    public string? StaffNotes { get; init; }
}
