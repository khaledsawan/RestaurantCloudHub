using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Reservations.DTOs;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Reservations.Commands.CreateReservation;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Result<ReservationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateReservationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<ReservationDto>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Result<ReservationDto>.Failure("User not authenticated");
        }

        var customerId = await _context.Customers
            .Where(c => c.UserId == _currentUserService.UserId.Value)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!customerId.HasValue)
        {
            return Result<ReservationDto>.Failure("Customer profile not found");
        }

        var tableExists = await _context.RestaurantTables
            .AnyAsync(t => t.Id == request.Reservation.TableId, cancellationToken);

        if (!tableExists)
        {
            return Result<ReservationDto>.Failure("Table not found");
        }

        var isBooked = await _context.Reservations
            .AnyAsync(r => r.TableId == request.Reservation.TableId
                           && r.ReservationDate == request.Reservation.ReservationDate
                           && r.ReservationTime == request.Reservation.ReservationTime
                           && r.Status != ReservationStatus.Cancelled,
                cancellationToken);

        if (isBooked)
        {
            return Result<ReservationDto>.Failure("Table is not available for that time");
        }

        var reservation = new Reservation
        {
            CustomerId = customerId.Value,
            TableId = request.Reservation.TableId,
            ReservationDate = request.Reservation.ReservationDate,
            ReservationTime = request.Reservation.ReservationTime,
            PartySize = request.Reservation.PartySize,
            SpecialRequests = request.Reservation.SpecialRequests,
            CustomerNotes = request.Reservation.CustomerNotes,
            Status = ReservationStatus.Pending
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<ReservationDto>.Success(new ReservationDto
        {
            Id = reservation.Id,
            ReservationDate = reservation.ReservationDate,
            ReservationTime = reservation.ReservationTime,
            PartySize = reservation.PartySize,
            Status = reservation.Status,
            ConfirmationCode = reservation.ConfirmationCode
        });
    }
}
