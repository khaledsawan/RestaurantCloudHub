using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Orders.Commands.AddOrderNote;
using RestaurantSystem.Application.Features.Orders.Commands.AssignDriver;
using RestaurantSystem.Application.Features.Orders.Commands.CancelOrder;
using RestaurantSystem.Application.Features.Orders.Commands.CreateOrder;
using RestaurantSystem.Application.Features.Orders.Commands.UpdateOrderStatus;
using RestaurantSystem.Application.Features.Orders.Queries.GetActiveOrders;
using RestaurantSystem.Application.Features.Orders.Queries.GetMyOrders;
using RestaurantSystem.Application.Features.Orders.Queries.GetOrderById;
using RestaurantSystem.Application.Features.Orders.Queries.GetOrderHistory;
using RestaurantSystem.WebAPI.Models;
using RestaurantSystem.WebAPI.Helpers;
using RestaurantSystem.Application.Features.Orders.DTOs;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded || result.Data == null)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrderById(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        if (result == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Order not found");
        }

        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<OrderSummaryDto>>> GetMyOrders()
    {
        var result = await _mediator.Send(new GetMyOrdersQuery());
        return Ok(result);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<OrderSummaryDto>>> GetActiveOrders()
    {
        var result = await _mediator.Send(new GetActiveOrdersQuery());
        return Ok(result);
    }

    [HttpGet("history")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<List<OrderSummaryDto>>> GetOrderHistory([FromQuery] GetOrderHistoryQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var command = new UpdateOrderStatusCommand
        {
            OrderId = id,
            Status = request.Status,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderRequest request)
    {
        var command = new CancelOrderCommand
        {
            OrderId = id,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("{id:int}/notes")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddOrderNote(int id, [FromBody] AddOrderNoteRequest request)
    {
        var command = new AddOrderNoteCommand
        {
            OrderId = id,
            NoteType = request.NoteType,
            Note = request.Note
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("{id:int}/assign-driver")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignDriver(int id, [FromBody] AssignDriverRequest request)
    {
        var command = new AssignDriverCommand
        {
            OrderId = id,
            StaffId = request.StaffId
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }
}
