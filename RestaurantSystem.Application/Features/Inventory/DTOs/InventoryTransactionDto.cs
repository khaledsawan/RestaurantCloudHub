using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Inventory.DTOs;

public class InventoryTransactionDto
{
    public long Id { get; set; }
    public int InventoryItemId { get; set; }
    public InventoryTransactionType TransactionType { get; set; }
    public decimal QuantityChange { get; set; }
    public decimal QuantityAfter { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
}
