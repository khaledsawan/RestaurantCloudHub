using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Customers.Commands.UpdateCustomerProfile;

public record UpdateCustomerProfileCommand : IRequest<Result>
{
    public string? Phone { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateOnly? DateOfBirth { get; init; }
    public string? ProfileImageUrl { get; init; }
}
