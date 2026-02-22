using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Reservations.DTOs;

namespace RestaurantSystem.Application.Features.Reservations.Queries.GetMyReservations;

public class GetMyReservationsQueryHandler : IRequestHandler<GetMyReservationsQuery, List<ReservationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyReservationsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<ReservationDto>> Handle(GetMyReservationsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return new List<ReservationDto>();
        }

        var customerId = await _context.Customers
            .Where(c => c.UserId == _currentUserService.UserId.Value)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!customerId.HasValue)
        {
            return new List<ReservationDto>();
        }

        return await _context.Reservations
            .AsNoTracking()
            .Where(r => r.CustomerId == customerId.Value)
            .OrderByDescending(r => r.ReservationDate)
            .ThenByDescending(r => r.ReservationTime)
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
