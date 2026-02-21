using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.DTOs;

public class CreateOrderDto
{
    public OrderType OrderType { get; set; }
    public int? DeliveryAddressId { get; set; }
    public string? CustomerNotes { get; set; }
    public string? KitchenNotes { get; set; }
    public string? DeliveryNotes { get; set; }
    public decimal? TipAmount { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
