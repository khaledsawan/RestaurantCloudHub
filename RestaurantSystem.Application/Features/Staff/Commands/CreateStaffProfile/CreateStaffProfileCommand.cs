using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Staff.Commands.CreateStaffProfile;

public record CreateStaffProfileCommand : IRequest<Result>
{
    public int UserId { get; init; }
    public string? Phone { get; init; }
    public string? ProfileImageUrl { get; init; }
}
