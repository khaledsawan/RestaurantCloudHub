namespace RestaurantSystem.Application.Features.Orders.DTOs;

public class OrderItemOptionDto
{
    public int OptionId { get; set; }
    public string? OptionGroupName { get; set; }
    public string? OptionName { get; set; }
    public decimal PriceAdjustment { get; set; }
}
