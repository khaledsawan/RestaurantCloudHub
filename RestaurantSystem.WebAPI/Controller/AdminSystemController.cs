using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.System.Commands.ClearCache;
using RestaurantSystem.Application.Features.System.Commands.UpsertSystemSetting;
using RestaurantSystem.Application.Features.System.DTOs;
using RestaurantSystem.Application.Features.System.Queries.GetAuditLogs;
using RestaurantSystem.Application.Features.System.Queries.GetSystemSettingByKey;
using RestaurantSystem.Application.Features.System.Queries.GetSystemSettings;
using RestaurantSystem.WebAPI.Helpers;
using RestaurantSystem.WebAPI.Models;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/admin/system")]
[Authorize(Roles = "Admin")]
public class AdminSystemController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminSystemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("cache/clear")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ClearCache()
    {
        var result = await _mediator.Send(new ClearCacheCommand());
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("audit-logs")]
    public async Task<ActionResult<PaginatedList<AuditLogDto>>> GetAuditLogs([FromQuery] GetAuditLogsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("settings")]
    public async Task<ActionResult<List<SystemSettingDto>>> GetSystemSettings()
    {
        var result = await _mediator.Send(new GetSystemSettingsQuery());
        return Ok(result);
    }

    [HttpGet("settings/{key}")]
    public async Task<ActionResult<SystemSettingDto>> GetSystemSetting(string key)
    {
        var result = await _mediator.Send(new GetSystemSettingByKeyQuery(key));
        if (result == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Setting not found");
        }

        return Ok(result);
    }

    [HttpPut("settings/{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpsertSystemSetting(string key, [FromBody] UpsertSystemSettingRequest request)
    {
        var command = new UpsertSystemSettingCommand
        {
            Key = key,
            Value = request.Value.GetRawText(),
            Description = request.Description
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }
}
