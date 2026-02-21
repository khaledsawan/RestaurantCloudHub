using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.WebAPI.Models;

public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
    public string? Notes { get; set; }
}
