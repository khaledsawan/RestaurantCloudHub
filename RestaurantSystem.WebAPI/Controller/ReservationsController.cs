using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Reservations.Commands.CancelReservation;
using RestaurantSystem.Application.Features.Reservations.Commands.ConfirmReservation;
using RestaurantSystem.Application.Features.Reservations.Commands.CreateReservation;
using RestaurantSystem.Application.Features.Reservations.Queries.GetAvailableTables;
using RestaurantSystem.Application.Features.Reservations.Queries.GetMyReservations;
using RestaurantSystem.Application.Features.Reservations.Queries.GetTodaysReservations;
using RestaurantSystem.Application.Features.Reservations.DTOs;
using RestaurantSystem.WebAPI.Helpers;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/reservations")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> CreateReservation([FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded || result.Data == null)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost("{id:int}/confirm")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ConfirmReservation(int id, [FromBody] ConfirmReservationCommand command)
    {
        var updated = command with { ReservationId = id };
        var result = await _mediator.Send(updated);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelReservation(int id, [FromBody] CancelReservationCommand command)
    {
        var updated = command with { ReservationId = id };
        var result = await _mediator.Send(updated);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<ReservationDto>>> GetMyReservations()
    {
        var result = await _mediator.Send(new GetMyReservationsQuery());
        return Ok(result);
    }

    [HttpGet("today")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<ReservationDto>>> GetTodaysReservations()
    {
        var result = await _mediator.Send(new GetTodaysReservationsQuery());
        return Ok(result);
    }

    [HttpGet("available-tables")]
    public async Task<ActionResult<List<TableAvailabilityDto>>> GetAvailableTables([FromQuery] GetAvailableTablesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
