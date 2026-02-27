using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Admin.Commands.UpdateAdminUserRole;

public class UpdateAdminUserRoleCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}
