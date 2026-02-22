using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Inventory.DTOs;

namespace RestaurantSystem.Application.Features.Inventory.Queries.GetLowStockItems;

public class GetLowStockItemsQueryHandler : IRequestHandler<GetLowStockItemsQuery, List<InventoryItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetLowStockItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<InventoryItemDto>> Handle(GetLowStockItemsQuery request, CancellationToken cancellationToken)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .Where(i => i.CurrentQuantity <= i.MinimumQuantity)
            .OrderBy(i => i.Name)
            .Select(i => new InventoryItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Sku = i.Sku,
                UnitOfMeasure = i.UnitOfMeasure,
                CurrentQuantity = i.CurrentQuantity,
                MinimumQuantity = i.MinimumQuantity,
                UnitCost = i.UnitCost,
                IsLowStock = true
            })
            .ToListAsync(cancellationToken);
    }
}
