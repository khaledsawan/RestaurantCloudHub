namespace RestaurantSystem.Application.Common.Interfaces;

/// <summary>
/// Interface for accessing current authenticated user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID (null if not authenticated)
    /// </summary>
    int? UserId { get; }
    
    /// <summary>
    /// Gets the current user's email
    /// </summary>
    string? Email { get; }
    
    /// <summary>
    /// Gets the current user's roles
    /// </summary>
    IEnumerable<string> Roles { get; }
    
    /// <summary>
    /// Checks if user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
    
    /// <summary>
    /// Checks if user has a specific role
    /// </summary>
    bool IsInRole(string role);
    
    /// <summary>
    /// Checks if user has any of the specified roles
    /// </summary>
    bool IsInAnyRole(params string[] roles);
}
