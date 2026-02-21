using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public string? ItemNotes { get; set; }
    public OrderItemStatus ItemStatus { get; set; } = OrderItemStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Order Order { get; set; } = null!;
    public virtual MenuItem MenuItem { get; set; } = null!;
    public virtual ICollection<OrderItemOption> SelectedOptions { get; set; } = new List<OrderItemOption>();
}
