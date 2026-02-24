namespace RestaurantSystem.WebAPI.Models.Responses;

public sealed class RegisterResponseDto
{
    public int UserId { get; init; }
    public bool RequiresEmailConfirmation { get; init; }
}
