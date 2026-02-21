namespace RestaurantSystem.Domain.Entities;

public class OrderItemOption
{
    public int OrderItemOptionId { get; set; }
    public int OrderItemId { get; set; }
    public int OptionId { get; set; }
    public string? OptionGroupName { get; set; }
    public string? OptionName { get; set; }
    public decimal PriceAdjustment { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual OrderItem OrderItem { get; set; } = null!;
    public virtual MenuItemOption Option { get; set; } = null!;
}
