namespace RestaurantSystem.WebAPI.Models.Responses;

public class TokenValidationDto
{
    public bool IsValid { get; set; }
    public int? UserId { get; set; }
}
