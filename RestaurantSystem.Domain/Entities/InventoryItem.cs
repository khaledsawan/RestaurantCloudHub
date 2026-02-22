using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities;

public class InventoryItem : BaseEntity, ISoftDeletable
{
    public int? InventoryCategoryId { get; set; }
    public string? Sku { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal CurrentQuantity { get; set; } = 0;
    public decimal MinimumQuantity { get; set; } = 0;
    public decimal ReorderQuantity { get; set; } = 0;
    public decimal UnitCost { get; set; } = 0;
    public string? SupplierName { get; set; }
    public string? SupplierContact { get; set; }
    public DateTime? LastRestockedAt { get; set; }
    public DateOnly? NextRestockDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    public virtual InventoryCategory? Category { get; set; }
    public virtual ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
}
