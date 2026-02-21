using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.DTOs;

public class OrderSummaryDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus OrderStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
