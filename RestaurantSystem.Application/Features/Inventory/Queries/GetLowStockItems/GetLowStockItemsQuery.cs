using MediatR;
using RestaurantSystem.Application.Features.Inventory.DTOs;

namespace RestaurantSystem.Application.Features.Inventory.Queries.GetLowStockItems;

public record GetLowStockItemsQuery : IRequest<List<InventoryItemDto>>;
