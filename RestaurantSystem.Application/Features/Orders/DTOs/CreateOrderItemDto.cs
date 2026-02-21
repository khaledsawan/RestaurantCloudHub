namespace RestaurantSystem.Application.Features.Orders.DTOs;

public class CreateOrderItemDto
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string? ItemNotes { get; set; }
    public List<int> SelectedOptionIds { get; set; } = new();
}
