using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.ChangeEmail;

public record ConfirmEmailChangeCommand : IRequest<Result>
{
    public string NewEmail { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}
