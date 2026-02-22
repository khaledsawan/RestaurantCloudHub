using MediatR;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Inventory.Commands.AdjustInventory;

public record AdjustInventoryCommand : IRequest<Result>
{
    public int InventoryItemId { get; init; }
    public decimal QuantityChange { get; init; }
    public string? Notes { get; init; }
}
