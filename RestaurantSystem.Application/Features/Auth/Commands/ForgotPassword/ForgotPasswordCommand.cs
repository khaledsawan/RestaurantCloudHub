using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand : IRequest<Result>
{
    public string Email { get; init; } = string.Empty;
}
