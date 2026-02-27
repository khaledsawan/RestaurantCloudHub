using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.WebAPI.Models;

public class UpdateUserRoleRequest
{
    public UserRole Role { get; set; }
}
