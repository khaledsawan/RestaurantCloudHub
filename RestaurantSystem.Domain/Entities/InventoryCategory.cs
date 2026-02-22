using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities;

public class InventoryCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public virtual ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();
}
