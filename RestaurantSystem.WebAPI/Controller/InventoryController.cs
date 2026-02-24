using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Inventory.Commands.AdjustInventory;
using RestaurantSystem.Application.Features.Inventory.Commands.RestockItem;
using RestaurantSystem.Application.Features.Inventory.Queries.GetInventoryReport;
using RestaurantSystem.Application.Features.Inventory.Queries.GetLowStockItems;
using RestaurantSystem.Application.Features.Inventory.DTOs;
using RestaurantSystem.WebAPI.Helpers;

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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AdjustInventory([FromBody] AdjustInventoryCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("restock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RestockItem([FromBody] RestockItemCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<List<InventoryItemDto>>> GetLowStockItems()
    {
        var result = await _mediator.Send(new GetLowStockItemsQuery());
        return Ok(result);
    }

    [HttpGet("report")]
    public async Task<ActionResult<List<InventoryTransactionDto>>> GetInventoryReport([FromQuery] GetInventoryReportQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
