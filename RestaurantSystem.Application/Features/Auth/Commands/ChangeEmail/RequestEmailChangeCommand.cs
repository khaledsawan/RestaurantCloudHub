using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.ChangeEmail;

public record RequestEmailChangeCommand : IRequest<Result>
{
    public string NewEmail { get; init; } = string.Empty;
}
