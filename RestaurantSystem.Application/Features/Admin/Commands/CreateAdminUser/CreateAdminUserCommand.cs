using MediatR;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Admin.DTOs;

namespace RestaurantSystem.Application.Features.Admin.Commands.CreateAdminUser;

public class CreateAdminUserCommand : IRequest<Result<AdminUserDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
}
