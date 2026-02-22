using MediatR;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Reservations.DTOs;

namespace RestaurantSystem.Application.Features.Reservations.Commands.CreateReservation;

public record CreateReservationCommand(CreateReservationDto Reservation) : IRequest<Result<ReservationDto>>;
