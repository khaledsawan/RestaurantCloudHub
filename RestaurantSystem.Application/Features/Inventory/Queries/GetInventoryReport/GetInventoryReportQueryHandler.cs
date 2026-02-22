using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Inventory.DTOs;

namespace RestaurantSystem.Application.Features.Inventory.Queries.GetInventoryReport;

public class GetInventoryReportQueryHandler : IRequestHandler<GetInventoryReportQuery, List<InventoryTransactionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InventoryTransactionDto>> Handle(GetInventoryReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InventoryTransactions.AsNoTracking().AsQueryable();

        if (request.DateFrom.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= request.DateTo.Value);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new InventoryTransactionDto
            {
                Id = t.TransactionId,
                InventoryItemId = t.InventoryItemId,
                TransactionType = t.TransactionType,
                QuantityChange = t.QuantityChange,
                QuantityAfter = t.QuantityAfter,
                CreatedAt = t.CreatedAt,
                Notes = t.Notes
            })
            .ToListAsync(cancellationToken);
    }
}
