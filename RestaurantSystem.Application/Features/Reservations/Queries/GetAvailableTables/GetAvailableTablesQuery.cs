using MediatR;
using RestaurantSystem.Application.Features.Reservations.DTOs;

namespace RestaurantSystem.Application.Features.Reservations.Queries.GetAvailableTables;

public record GetAvailableTablesQuery : IRequest<List<TableAvailabilityDto>>
{
    public DateOnly ReservationDate { get; init; }
    public TimeOnly ReservationTime { get; init; }
    public int PartySize { get; init; }
}
