namespace RestaurantSystem.Application.Common.Models;

/// <summary>
/// Result of authentication operation
/// </summary>
public class AuthResult
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
