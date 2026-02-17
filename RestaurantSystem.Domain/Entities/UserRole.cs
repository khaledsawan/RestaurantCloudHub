using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities.Identity;

/// <summary>
/// User role assignment (many-to-many)
/// </summary>
public class UserRole : BaseEntity
{
    public int UserId { get; set; }
    public string RoleName { get; set; } = string.Empty; // "Admin", "Customer", "Staff", etc.
    
    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
}
