using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Reservations.Commands.CancelReservation;

public class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CancelReservationCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == request.ReservationId, cancellationToken);

        if (reservation == null)
        {
            return Result.Failure("Reservation not found");
        }

        var isStaff = _currentUserService.IsInAnyRole("Admin", "Manager");
        if (!isStaff)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure("User not authenticated");
            }

            var customerId = await _context.Customers
                .Where(c => c.UserId == _currentUserService.UserId.Value)
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (!customerId.HasValue || reservation.CustomerId != customerId.Value)
            {
                return Result.Failure("Not authorized to cancel this reservation");
            }
        }

        reservation.Status = ReservationStatus.Cancelled;
        reservation.CancelledAt = DateTime.UtcNow;
        reservation.CancellationReason = request.Reason;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
