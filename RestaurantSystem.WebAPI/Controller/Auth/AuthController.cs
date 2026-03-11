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
using RestaurantSystem.WebAPI.Helpers;
using RestaurantSystem.WebAPI.Models.Responses;
using RestaurantSystem.Application.Common.Models;
using Microsoft.AspNetCore.RateLimiting;
using Infrastructure.RateLimiting;

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
    [EnableRateLimiting(RateLimiterPolicies.Register)]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new RegisterResponseDto { UserId = result.Data, RequiresEmailConfirmation = true });
    }

    /// <summary>
    /// Login and get tokens
    /// </summary>
    [HttpPost("login")]
    [EnableRateLimiting(RateLimiterPolicies.Login)]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded || result.Data == null)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResult>> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded || result.Data == null)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Confirm email (POST)
    /// </summary>
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Email confirmed successfully" });
    }

    /// <summary>
    /// Resend confirmation email
    /// </summary>
    [HttpPost("resend-confirmation")]
    [EnableRateLimiting(RateLimiterPolicies.SendOtp)]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResendConfirmation([FromBody] ResendConfirmationCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Confirmation email sent" });
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Password changed successfully" });
    }

    /// <summary>
    /// Forgot password - send reset code
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new MessageResponseDto { Message = "If the email exists, a reset code has been sent." });
    }

    /// <summary>
    /// Reset password with code
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<MessageResponseDto>> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Password reset successfully" });
    }

    /// <summary>
    /// Request email change (send code)
    /// </summary>
    [HttpPost("change-email-request")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> ChangeEmailRequest([FromBody] RequestEmailChangeCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Confirmation code sent" });
    }

    /// <summary>
    /// Confirm email change
    /// </summary>
    [HttpPost("change-email-confirm")]
    [Authorize]
    public async Task<ActionResult<MessageResponseDto>> ChangeEmailConfirm([FromBody] ConfirmEmailChangeCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Email updated successfully" });
    }

    /// <summary>
    /// Delete account (soft delete)
    /// </summary>
    [HttpPost("delete-account")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAccount()
    {
        var result = await _mediator.Send(new DeleteAccountCommand());
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    /// <summary>
    /// Admin: update user active status
    /// </summary>
    [HttpPatch("admin/users/{userId:int}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetUserStatus(int userId, [FromBody] SetUserActiveStatusRequest request)
    {
        var command = new SetUserActiveStatusCommand(userId, request.IsActive);
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    /// <summary>
    /// Get current user
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUserDto>> Me()
    {
        var result = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(result);
    }

    /// <summary>
    /// Validate token
    /// </summary>
    [HttpPost("validate-token")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenValidationDto>> ValidateToken([FromBody] ValidateTokenQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    /// <summary>
    /// Resend confirmation email
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }
}
