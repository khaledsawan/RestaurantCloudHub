using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.ResendConfirmation;

public record ResendConfirmationCommand : IRequest<Result>
{
    public string Email { get; init; } = string.Empty;
}
