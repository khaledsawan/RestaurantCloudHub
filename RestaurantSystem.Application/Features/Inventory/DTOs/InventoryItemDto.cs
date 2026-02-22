namespace RestaurantSystem.Application.Features.Inventory.DTOs;

public class InventoryItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Sku { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal CurrentQuantity { get; set; }
    public decimal MinimumQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public bool IsLowStock { get; set; }
}
