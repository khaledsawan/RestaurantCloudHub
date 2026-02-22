using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Inventory.Commands.RestockItem;

public record RestockItemCommand : IRequest<Result>
{
    public int InventoryItemId { get; init; }
    public decimal QuantityAdded { get; init; }
    public decimal? UnitCost { get; init; }
    public string? Notes { get; init; }
}
