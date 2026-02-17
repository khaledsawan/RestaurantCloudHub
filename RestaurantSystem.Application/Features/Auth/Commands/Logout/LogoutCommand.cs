using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.Logout;

public record LogoutCommand : IRequest<Result>
{
    public string RefreshToken { get; init; } = string.Empty;
}
