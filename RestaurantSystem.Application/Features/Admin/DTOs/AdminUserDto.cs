namespace RestaurantSystem.Application.Features.Admin.DTOs;

public class AdminUserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
}
