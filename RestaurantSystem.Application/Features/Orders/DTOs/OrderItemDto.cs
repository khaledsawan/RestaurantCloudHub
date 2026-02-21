namespace RestaurantSystem.Application.Features.Orders.DTOs;

public class OrderItemDto
{
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
    public string? ItemNotes { get; set; }
    public List<OrderItemOptionDto> Options { get; set; } = new();
}
