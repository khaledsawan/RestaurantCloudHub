using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Reservations.DTOs;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Reservations.Queries.GetAvailableTables;

public class GetAvailableTablesQueryHandler : IRequestHandler<GetAvailableTablesQuery, List<TableAvailabilityDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAvailableTablesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TableAvailabilityDto>> Handle(GetAvailableTablesQuery request, CancellationToken cancellationToken)
    {
        var reservedTableIds = await _context.Reservations
            .Where(r => r.ReservationDate == request.ReservationDate
                        && r.ReservationTime == request.ReservationTime
                        && r.Status != ReservationStatus.Cancelled)
            .Select(r => r.TableId)
            .ToListAsync(cancellationToken);

        return await _context.RestaurantTables
            .AsNoTracking()
            .Where(t => t.Capacity >= request.PartySize)
            .OrderBy(t => t.TableNumber)
            .Select(t => new TableAvailabilityDto
            {
                TableId = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                IsAvailable = !reservedTableIds.Contains(t.Id)
            })
            .ToListAsync(cancellationToken);
    }
}
