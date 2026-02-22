using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Reservations.Commands.ConfirmReservation;

public class ConfirmReservationCommandHandler : IRequestHandler<ConfirmReservationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public ConfirmReservationCommandHandler(IApplicationDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == request.ReservationId, cancellationToken);

        if (reservation == null)
        {
            return Result.Failure("Reservation not found");
        }

        reservation.Status = ReservationStatus.Confirmed;
        reservation.StaffNotes = request.StaffNotes;
        reservation.ConfirmationCode ??= Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

        await _context.SaveChangesAsync(cancellationToken);

        await _emailService.SendReservationConfirmationAsync(
            reservation.Customer.Email,
            reservation.ConfirmationCode,
            reservation.ReservationDate.ToDateTime(reservation.ReservationTime));

        return Result.Success();
    }
}
