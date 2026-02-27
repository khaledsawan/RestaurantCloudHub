using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Admin.Commands.UpdateAdminUserStatus;

public class UpdateAdminUserStatusCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public bool IsActive { get; set; }
}
