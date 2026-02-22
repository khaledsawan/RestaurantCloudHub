using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Payments.Commands.ProcessPayment;
using RestaurantSystem.Application.Features.Payments.Commands.RefundPayment;
using RestaurantSystem.Application.Features.Payments.Queries.GetPaymentHistory;

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
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    [HttpPost("refund")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RefundPayment([FromBody] RefundPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Payment refunded" });
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetPaymentHistory([FromQuery] GetPaymentHistoryQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
