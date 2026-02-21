using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Orders.DTOs;

namespace RestaurantSystem.Application.Features.Orders.Queries.GetOrderHistory;

public class GetOrderHistoryQueryHandler : IRequestHandler<GetOrderHistoryQuery, List<OrderSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrderHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderSummaryDto>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Orders.AsNoTracking().AsQueryable();

        if (request.DateFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= request.DateTo.Value);
        }

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderStatus = o.OrderStatus,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
