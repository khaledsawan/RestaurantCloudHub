namespace RestaurantSystem.Application.Common.Interfaces;

/// <summary>
/// Interface for the application database context
/// Used for dependency inversion - Application layer doesn't depend on Infrastructure
/// NO references to EntityFramework - just contracts!
/// </summary>
public interface IApplicationDbContext
{
    // Add entity collections as you create entities
    // These will return IQueryable in implementations
    // Example:
    // IQueryable<Customer> Customers { get; }
    // IQueryable<Order> Orders { get; }
    // IQueryable<MenuItem> MenuItems { get; }
    
    /// <summary>
    /// Save changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}