namespace RestaurantSystem.Domain.Entities;

public class MenuItemIngredient
{
    public int MenuItemIngredientId { get; set; }
    public int ItemId { get; set; }
    public int InventoryItemId { get; set; }
    public decimal QuantityRequired { get; set; }

    public virtual MenuItem MenuItem { get; set; } = null!;
    public virtual InventoryItem InventoryItem { get; set; } = null!;
}
