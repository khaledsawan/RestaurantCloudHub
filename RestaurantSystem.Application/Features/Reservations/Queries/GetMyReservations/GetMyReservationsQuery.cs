using MediatR;
using RestaurantSystem.Application.Features.Reservations.DTOs;

namespace RestaurantSystem.Application.Features.Reservations.Queries.GetMyReservations;

public record GetMyReservationsQuery : IRequest<List<ReservationDto>>;
