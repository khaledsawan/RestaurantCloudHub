using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.DTOs;

public class OrderDetailDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus OrderStatus { get; set; }
    public OrderType OrderType { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public string? CustomerNotes { get; set; }
    public string? KitchenNotes { get; set; }
    public string? DeliveryNotes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();
}
