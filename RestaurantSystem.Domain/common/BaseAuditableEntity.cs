namespace RestaurantSystem.Domain.Common;

/// <summary>
/// Base entity with audit information (who created/updated)
/// </summary>
public abstract class BaseAuditableEntity : BaseEntity
{
    public int? CreatedById { get; set; }
    
    public int? UpdatedById { get; set; }
}
