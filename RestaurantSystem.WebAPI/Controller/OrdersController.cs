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
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded || result.Data == null)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var result = await _mediator.Send(new GetMyOrdersQuery());
        return Ok(result);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetActiveOrders()
    {
        var result = await _mediator.Send(new GetActiveOrdersQuery());
        return Ok(result);
    }

    [HttpGet("history")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetOrderHistory([FromQuery] GetOrderHistoryQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin,Manager")]
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
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Order status updated" });
    }

    [HttpPost("{id:int}/cancel")]
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
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Order cancelled" });
    }

    [HttpPost("{id:int}/notes")]
    [Authorize(Roles = "Admin,Manager")]
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
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Order note added" });
    }

    [HttpPost("{id:int}/assign-driver")]
    [Authorize(Roles = "Admin,Manager")]
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
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Driver assigned" });
    }
}
