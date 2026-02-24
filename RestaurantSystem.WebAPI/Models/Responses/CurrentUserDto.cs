namespace RestaurantSystem.WebAPI.Models.Responses;

public class CurrentUserDto
{
    public bool IsAuthenticated { get; set; }
    public int? UserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool EmailConfirmed { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
