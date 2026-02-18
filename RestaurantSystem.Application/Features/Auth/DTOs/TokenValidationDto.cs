namespace RestaurantSystem.Application.Features.Auth.DTOs;

public class TokenValidationDto
{
    public bool IsValid { get; set; }
    public int? UserId { get; set; }
}
