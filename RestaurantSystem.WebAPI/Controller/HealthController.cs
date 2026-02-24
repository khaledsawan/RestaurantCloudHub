using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RestaurantSystem.WebAPI.Models.Responses;
using System.Linq;

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
    [ProducesResponseType(typeof(HealthStatusResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthStatusResponseDto), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<HealthStatusResponseDto>> Live()
    {
        var result = await _healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains("live"));

        return result.Status == HealthStatus.Healthy
            ? Ok(new HealthStatusResponseDto { Status = "Healthy", Checks = result.Entries.ToDictionary(k => k.Key, v => v.Value) })
            : StatusCode(503, new HealthStatusResponseDto { Status = result.Status.ToString(), Checks = result.Entries.ToDictionary(k => k.Key, v => v.Value) });
    }

    /// <summary>
    /// Check if the application is ready (including database)
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(typeof(HealthStatusResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthStatusResponseDto), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<HealthStatusResponseDto>> Ready()
    {
        var result = await _healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains("ready"));

        return result.Status == HealthStatus.Healthy
            ? Ok(new HealthStatusResponseDto { Status = "Healthy", Checks = result.Entries.ToDictionary(k => k.Key, v => v.Value) })
            : StatusCode(503, new HealthStatusResponseDto { Status = result.Status.ToString(), Checks = result.Entries.ToDictionary(k => k.Key, v => v.Value) });
    }

    /// <summary>
    /// Check overall application health
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthStatusWithDurationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthStatusWithDurationResponseDto), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<HealthStatusWithDurationResponseDto>> GetHealth()
    {
        var result = await _healthCheckService.CheckHealthAsync();

        return result.Status == HealthStatus.Healthy
            ? Ok(new HealthStatusWithDurationResponseDto
            {
                Status = "Healthy",
                Checks = result.Entries.ToDictionary(k => k.Key, v => v.Value),
                TotalDuration = result.TotalDuration
            })
            : StatusCode(503, new HealthStatusWithDurationResponseDto
            {
                Status = result.Status.ToString(),
                Checks = result.Entries.ToDictionary(k => k.Key, v => v.Value),
                TotalDuration = result.TotalDuration
            });
    }
}
