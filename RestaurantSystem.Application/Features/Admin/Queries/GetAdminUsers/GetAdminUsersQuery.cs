using MediatR;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Admin.DTOs;

namespace RestaurantSystem.Application.Features.Admin.Queries.GetAdminUsers;

public class GetAdminUsersQuery : IRequest<PaginatedList<AdminUserDto>>
{
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
    public bool? EmailConfirmed { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
