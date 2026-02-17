namespace RestaurantSystem.Application.Features.Auth.Commands.Register;


using MediatR;
using RestaurantSystem.Application.Common.Models;

public record RegisterCommand : IRequest<Result<int>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}