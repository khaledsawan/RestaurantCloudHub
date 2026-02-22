namespace RestaurantSystem.WebAPI.Models;

public class CreateStaffProfileRequest
{
    public int UserId { get; set; }
    public string? Phone { get; set; }
    public string? ProfileImageUrl { get; set; }
}
