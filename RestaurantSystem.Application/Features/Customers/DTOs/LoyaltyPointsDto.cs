namespace RestaurantSystem.Application.Features.Customers.DTOs;

public class LoyaltyPointsDto
{
    public int LoyaltyPoints { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
}
