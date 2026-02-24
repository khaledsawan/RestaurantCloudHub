using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Payments.Commands.ProcessPayment;
using RestaurantSystem.Application.Features.Payments.Commands.RefundPayment;
using RestaurantSystem.Application.Features.Payments.Queries.GetPaymentHistory;
using RestaurantSystem.Application.Features.Payments.DTOs;
using RestaurantSystem.WebAPI.Helpers;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("process")]
    public async Task<ActionResult<PaymentResponseDto>> ProcessPayment([FromBody] ProcessPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded || result.Data == null)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost("refund")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RefundPayment([FromBody] RefundPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<PaymentDto>>> GetPaymentHistory([FromQuery] GetPaymentHistoryQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
