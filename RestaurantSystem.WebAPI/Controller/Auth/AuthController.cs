using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Auth.Commands.Register;
using RestaurantSystem.Application.Features.Auth.Commands.Login;
using RestaurantSystem.Application.Features.Auth.Commands.RefreshToken;
using RestaurantSystem.Application.Features.Auth.Commands.ConfirmEmail;
using RestaurantSystem.Application.Features.Auth.Commands.ResendConfirmation;
using RestaurantSystem.Application.Features.Auth.Commands.Logout;
using RestaurantSystem.Application.Features.Auth.Commands.ChangePassword;
using RestaurantSystem.Application.Features.Auth.Commands.ForgotPassword;
using RestaurantSystem.Application.Features.Auth.Commands.ResetPassword;
using RestaurantSystem.Application.Features.Auth.Commands.ChangeEmail;
using RestaurantSystem.Application.Features.Auth.Commands.DeleteAccount;
using RestaurantSystem.Application.Features.Auth.Commands.SetUserActiveStatus;
using RestaurantSystem.WebAPI.Models;
using RestaurantSystem.Application.Features.Auth.Queries.GetCurrentUser;
using RestaurantSystem.Application.Features.Auth.Queries.ValidateToken;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { userId = result.Data, requiresEmailConfirmation = true });
    }

    /// <summary>
    /// Login and get tokens
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Confirm email (POST)
    /// </summary>
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Email confirmed successfully" });
    }

    /// <summary>
    /// Resend confirmation email
    /// </summary>
    [HttpPost("resend-confirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Confirmation email sent" });
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Password changed successfully" });
    }

    /// <summary>
    /// Forgot password - send reset code
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "If the email exists, a reset code has been sent." });
    }

    /// <summary>
    /// Reset password with code
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Password reset successfully" });
    }

    /// <summary>
    /// Request email change (send code)
    /// </summary>
    [HttpPost("change-email-request")]
    [Authorize]
    public async Task<IActionResult> ChangeEmailRequest([FromBody] RequestEmailChangeCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Confirmation code sent" });
    }

    /// <summary>
    /// Confirm email change
    /// </summary>
    [HttpPost("change-email-confirm")]
    [Authorize]
    public async Task<IActionResult> ChangeEmailConfirm([FromBody] ConfirmEmailChangeCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Email updated successfully" });
    }

    /// <summary>
    /// Delete account (soft delete)
    /// </summary>
    [HttpPost("delete-account")]
    [Authorize]
    public async Task<IActionResult> DeleteAccount()
    {
        var result = await _mediator.Send(new DeleteAccountCommand());
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Account deleted" });
    }

    /// <summary>
    /// Admin: update user active status
    /// </summary>
    [HttpPatch("admin/users/{userId:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetUserStatus(int userId, [FromBody] SetUserActiveStatusRequest request)
    {
        var command = new SetUserActiveStatusCommand(userId, request.IsActive);
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "User status updated" });
    }

    /// <summary>
    /// Get current user
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var result = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(result);
    }

    /// <summary>
    /// Validate token
    /// </summary>
    [HttpPost("validate-token")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    /// <summary>
    /// Resend confirmation email
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok();
    }
}
