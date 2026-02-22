using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Reservations.Commands.CancelReservation;
using RestaurantSystem.Application.Features.Reservations.Commands.ConfirmReservation;
using RestaurantSystem.Application.Features.Reservations.Commands.CreateReservation;
using RestaurantSystem.Application.Features.Reservations.Queries.GetAvailableTables;
using RestaurantSystem.Application.Features.Reservations.Queries.GetMyReservations;
using RestaurantSystem.Application.Features.Reservations.Queries.GetTodaysReservations;

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
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    [HttpPost("{id:int}/confirm")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ConfirmReservation(int id, [FromBody] ConfirmReservationCommand command)
    {
        var updated = command with { ReservationId = id };
        var result = await _mediator.Send(updated);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Reservation confirmed" });
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelReservation(int id, [FromBody] CancelReservationCommand command)
    {
        var updated = command with { ReservationId = id };
        var result = await _mediator.Send(updated);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Reservation cancelled" });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyReservations()
    {
        var result = await _mediator.Send(new GetMyReservationsQuery());
        return Ok(result);
    }

    [HttpGet("today")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetTodaysReservations()
    {
        var result = await _mediator.Send(new GetTodaysReservationsQuery());
        return Ok(result);
    }

    [HttpGet("available-tables")]
    public async Task<IActionResult> GetAvailableTables([FromQuery] GetAvailableTablesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
