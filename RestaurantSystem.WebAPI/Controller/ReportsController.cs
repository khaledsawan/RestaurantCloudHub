using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Reports.Queries.GetCustomerAnalytics;
using RestaurantSystem.Application.Features.Reports.Queries.GetPopularItems;
using RestaurantSystem.Application.Features.Reports.Queries.GetSalesReport;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin,Manager")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("sales")]
    public async Task<IActionResult> GetSalesReport([FromQuery] GetSalesReportQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("popular-items")]
    public async Task<IActionResult> GetPopularItems([FromQuery] GetPopularItemsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("customer-analytics")]
    public async Task<IActionResult> GetCustomerAnalytics()
    {
        var result = await _mediator.Send(new GetCustomerAnalyticsQuery());
        return Ok(result);
    }
}
