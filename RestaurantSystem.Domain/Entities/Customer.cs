using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Entities.Identity;

namespace RestaurantSystem.Domain.Entities;

public class Customer : BaseAuditableEntity, ISoftDeletable
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? ProfileImageUrl { get; set; }
    public int LoyaltyPoints { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal? AverageRating { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
}
