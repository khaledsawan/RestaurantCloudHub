namespace RestaurantSystem.Domain.Common;

/// <summary>
/// Interface for entities that support soft delete
/// </summary>
public interface ISoftDeletable
{
    DateTime? DeletedAt { get; set; }
    
    bool IsDeleted => DeletedAt.HasValue;
}
