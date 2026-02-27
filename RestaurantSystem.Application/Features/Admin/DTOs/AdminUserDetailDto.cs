namespace RestaurantSystem.Application.Features.Admin.DTOs;

public class AdminUserDetailDto : AdminUserDto
{
    public int? CustomerId { get; set; }
    public bool? CustomerIsActive { get; set; }
    public DateTime? CustomerDeletedAt { get; set; }

    public int? StaffId { get; set; }
    public bool? StaffIsActive { get; set; }
    public DateTime? StaffDeletedAt { get; set; }
}
