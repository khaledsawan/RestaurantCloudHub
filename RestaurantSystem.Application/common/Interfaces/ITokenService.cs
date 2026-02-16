namespace RestaurantSystem.Application.Common.Interfaces;

/// <summary>
/// Interface for JWT token operations
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generate JWT access token
    /// </summary>
    string GenerateAccessToken(int userId, string email, IEnumerable<string> roles);
    
    /// <summary>
    /// Generate refresh token
    /// </summary>
    string GenerateRefreshToken();
    
    /// <summary>
    /// Validate token and extract user ID
    /// </summary>
    int? ValidateToken(string token);
    
    /// <summary>
    /// Get token expiration time in minutes
    /// </summary>
    int GetTokenExpirationMinutes();
}