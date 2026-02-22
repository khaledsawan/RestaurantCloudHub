using MediatR;
using RestaurantSystem.Application.Features.Reservations.DTOs;

namespace RestaurantSystem.Application.Features.Reservations.Queries.GetTodaysReservations;

public record GetTodaysReservationsQuery : IRequest<List<ReservationDto>>;
