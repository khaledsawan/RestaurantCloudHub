using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Reservations.DTOs;

namespace RestaurantSystem.Application.Features.Reservations.Queries.GetTodaysReservations;

public class GetTodaysReservationsQueryHandler : IRequestHandler<GetTodaysReservationsQuery, List<ReservationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTodaysReservationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReservationDto>> Handle(GetTodaysReservationsQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return await _context.Reservations
            .AsNoTracking()
            .Where(r => r.ReservationDate == today)
            .OrderBy(r => r.ReservationTime)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                ReservationDate = r.ReservationDate,
                ReservationTime = r.ReservationTime,
                PartySize = r.PartySize,
                Status = r.Status,
                ConfirmationCode = r.ConfirmationCode
            })
            .ToListAsync(cancellationToken);
    }
}
