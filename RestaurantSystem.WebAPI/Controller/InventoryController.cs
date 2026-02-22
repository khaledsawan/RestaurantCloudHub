using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Inventory.Commands.AdjustInventory;
using RestaurantSystem.Application.Features.Inventory.Commands.RestockItem;
using RestaurantSystem.Application.Features.Inventory.Queries.GetInventoryReport;
using RestaurantSystem.Application.Features.Inventory.Queries.GetLowStockItems;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize(Roles = "Admin,Manager")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("adjust")]
    public async Task<IActionResult> AdjustInventory([FromBody] AdjustInventoryCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Inventory adjusted" });
    }

    [HttpPost("restock")]
    public async Task<IActionResult> RestockItem([FromBody] RestockItemCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Inventory restocked" });
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockItems()
    {
        var result = await _mediator.Send(new GetLowStockItemsQuery());
        return Ok(result);
    }

    [HttpGet("report")]
    public async Task<IActionResult> GetInventoryReport([FromQuery] GetInventoryReportQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
