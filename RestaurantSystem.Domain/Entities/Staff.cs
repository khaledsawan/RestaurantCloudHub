using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Entities.Identity;

namespace RestaurantSystem.Domain.Entities;

public class Staff : BaseAuditableEntity, ISoftDeletable
{
    public int UserId { get; set; }
    public string? Phone { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
}
