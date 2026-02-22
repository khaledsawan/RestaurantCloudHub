using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Reports.DTOs;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Reports.Queries.GetPopularItems;

public class GetPopularItemsQueryHandler : IRequestHandler<GetPopularItemsQuery, List<PopularItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPopularItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PopularItemDto>> Handle(GetPopularItemsQuery request, CancellationToken cancellationToken)
    {
        var ordersQuery = _context.Orders
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(o => o.OrderStatus == OrderStatus.Completed);

        if (request.DateFrom.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.CreatedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.CreatedAt <= request.DateTo.Value);
        }

        var orderIds = ordersQuery.Select(o => o.Id);

        return await _context.OrderItems
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(i => orderIds.Contains(i.OrderId))
            .GroupBy(i => new { i.ItemId, i.MenuItem.Name })
            .Select(g => new PopularItemDto
            {
                ItemId = g.Key.ItemId,
                Name = g.Key.Name,
                QuantitySold = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);
    }
}
