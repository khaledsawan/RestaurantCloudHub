using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Admin.Commands.CreateAdminUser;
using RestaurantSystem.Application.Features.Admin.Commands.DeleteAdminUser;
using RestaurantSystem.Application.Features.Admin.Commands.UpdateAdminUserRole;
using RestaurantSystem.Application.Features.Admin.Commands.UpdateAdminUserStatus;
using RestaurantSystem.Application.Features.Admin.DTOs;
using RestaurantSystem.Application.Features.Admin.Queries.GetAdminUserById;
using RestaurantSystem.Application.Features.Admin.Queries.GetAdminUsers;
using RestaurantSystem.WebAPI.Helpers;
using RestaurantSystem.WebAPI.Models;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin,Manager")]
public class AdminUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<AdminUserDto>>> GetUsers([FromQuery] GetAdminUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdminUserDetailDto>> GetUserById(int id)
    {
        var result = await _mediator.Send(new GetAdminUserByIdQuery(id));
        if (result == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "User not found");
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AdminUserDto>> CreateUser([FromBody] CreateAdminUserRequest request)
    {
        var command = new CreateAdminUserCommand
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role.ToString()
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded || result.Data == null)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPatch("{id:int}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleRequest request)
    {
        var result = await _mediator.Send(new UpdateAdminUserRoleCommand
        {
            UserId = id,
            Role = request.Role.ToString()
        });

        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusRequest request)
    {
        var result = await _mediator.Send(new UpdateAdminUserStatusCommand
        {
            UserId = id,
            IsActive = request.IsActive
        });

        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _mediator.Send(new DeleteAdminUserCommand(id));
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }
}
