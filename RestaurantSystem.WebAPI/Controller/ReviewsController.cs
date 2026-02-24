using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Reviews.Commands.CreateReview;
using RestaurantSystem.Application.Features.Reviews.Commands.RespondToReview;
using RestaurantSystem.Application.Features.Reviews.Queries.GetMenuItemReviews;
using RestaurantSystem.Application.Features.Reviews.DTOs;
using RestaurantSystem.WebAPI.Helpers;
using RestaurantSystem.WebAPI.Models.Responses;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> CreateReview([FromBody] CreateReviewCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Review submitted" });
    }

    [HttpPost("{id:int}/respond")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RespondToReview(int id, [FromBody] RespondToReviewCommand command)
    {
        var updated = command with { ReviewId = id };
        var result = await _mediator.Send(updated);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("menu-items/{menuItemId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewDto>>> GetMenuItemReviews(int menuItemId)
    {
        var result = await _mediator.Send(new GetMenuItemReviewsQuery(menuItemId));
        return Ok(result);
    }
}
