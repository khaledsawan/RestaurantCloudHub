using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand : IRequest<Result>
{
    public string Email { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
