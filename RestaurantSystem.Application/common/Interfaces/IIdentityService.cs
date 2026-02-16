using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<int>> RegisterAsync(string email, string password, string firstName, string lastName, string role = "Customer");
    Task<Result<AuthResult>> LoginAsync(string email, string password);
    Task<Result<AuthResult>> RefreshTokenAsync(string refreshToken);
    Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<Result<UserDto>> GetUserByIdAsync(int userId);
    Task<Result<UserDto>> GetUserByEmailAsync(string email);
    Task<bool> UserExistsAsync(int userId);
    Task<bool> EmailExistsAsync(string email);
    Task<Result> RevokeTokenAsync(string refreshToken);
    Task<Result> AssignRoleAsync(int userId, string role);
    Task<Result> RemoveRoleAsync(int userId, string role);
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
}