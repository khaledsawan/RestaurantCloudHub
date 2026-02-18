namespace RestaurantSystem.Application.Common.Interfaces;

/// <summary>
/// Interface for DateTime operations
/// Allows mocking time in tests
/// </summary>
public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
