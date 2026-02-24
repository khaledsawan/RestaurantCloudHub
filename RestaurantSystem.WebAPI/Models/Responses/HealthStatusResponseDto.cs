using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RestaurantSystem.WebAPI.Models.Responses;

public sealed class HealthStatusResponseDto
{
    public string Status { get; init; } = string.Empty;
    public Dictionary<string, HealthReportEntry> Checks { get; init; } = new();
}
