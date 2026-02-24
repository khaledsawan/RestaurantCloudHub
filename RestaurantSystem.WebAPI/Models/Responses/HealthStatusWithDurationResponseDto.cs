using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RestaurantSystem.WebAPI.Models.Responses;

public sealed class HealthStatusWithDurationResponseDto
{
    public string Status { get; init; } = string.Empty;
    public Dictionary<string, HealthReportEntry> Checks { get; init; } = new();
    public TimeSpan TotalDuration { get; init; }
}
