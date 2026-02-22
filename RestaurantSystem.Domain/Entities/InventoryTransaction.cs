using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class InventoryTransaction
{
    public long TransactionId { get; set; }
    public int InventoryItemId { get; set; }
    public InventoryTransactionType TransactionType { get; set; }
    public decimal QuantityChange { get; set; }
    public decimal QuantityAfter { get; set; }
    public decimal? UnitCost { get; set; }
    public int? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public string? Notes { get; set; }
    public int? StaffId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual InventoryItem InventoryItem { get; set; } = null!;
    public virtual Staff? Staff { get; set; }
}
