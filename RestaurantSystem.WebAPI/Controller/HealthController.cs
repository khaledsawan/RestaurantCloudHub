using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RestaurantSystem.Controllers;

[ApiController]
[Route("api/health")]
[ApiExplorerSettings(GroupName = "health")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Check if the application is alive
    /// </summary>
    [HttpGet("live")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Live()
    {
        var result = await _healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains("live"));

        return result.Status == HealthStatus.Healthy
            ? Ok(new { status = "Healthy", checks = result.Entries })
            : StatusCode(503, new { status = result.Status.ToString(), checks = result.Entries });
    }

    /// <summary>
    /// Check if the application is ready (including database)
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Ready()
    {
        var result = await _healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains("ready"));

        return result.Status == HealthStatus.Healthy
            ? Ok(new { status = "Healthy", checks = result.Entries })
            : StatusCode(503, new { status = result.Status.ToString(), checks = result.Entries });
    }

    /// <summary>
    /// Check overall application health
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealth()
    {
        var result = await _healthCheckService.CheckHealthAsync();

        return result.Status == HealthStatus.Healthy
            ? Ok(new
            {
                status = "Healthy",
                checks = result.Entries,
                totalDuration = result.TotalDuration
            })
            : StatusCode(503, new
            {
                status = result.Status.ToString(),
                checks = result.Entries,
                totalDuration = result.TotalDuration
            });
    }
}