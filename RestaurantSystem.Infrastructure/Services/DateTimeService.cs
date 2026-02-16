using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.Infrastructure.Services;

/// <summary>
/// Service for DateTime operations
/// </summary>
public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;

    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}