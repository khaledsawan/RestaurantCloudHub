
namespace RestaurantSystem.Domain.Common;

/// <summary>
/// Base class for domain events
/// Domain events represent something that happened in the domain
/// </summary>
public abstract class BaseEvent
{
    public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
}
