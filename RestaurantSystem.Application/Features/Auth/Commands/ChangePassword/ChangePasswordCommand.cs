using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand : IRequest<Result>
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
