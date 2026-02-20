namespace RestaurantSystem.Application.Features.Customers.DTOs;

public class CustomerProfileDto
{
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsVerified { get; set; }
    public int LoyaltyPoints { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal? AverageRating { get; set; }
    public DateTime CreatedAt { get; set; }
}
