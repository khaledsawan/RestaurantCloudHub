using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Staff.Commands.CreateStaffProfile;
using RestaurantSystem.WebAPI.Models;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/staff")]
[Authorize(Roles = "Admin,Manager")]
public class StaffController : ControllerBase
{
    private readonly IMediator _mediator;

    public StaffController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffProfileRequest request)
    {
        var command = new CreateStaffProfileCommand
        {
            UserId = request.UserId,
            Phone = request.Phone,
            ProfileImageUrl = request.ProfileImageUrl
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Staff profile created" });
    }
}
