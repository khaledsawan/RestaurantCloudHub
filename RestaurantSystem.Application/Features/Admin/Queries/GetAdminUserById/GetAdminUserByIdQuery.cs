using MediatR;
using RestaurantSystem.Application.Features.Admin.DTOs;

namespace RestaurantSystem.Application.Features.Admin.Queries.GetAdminUserById;

public record GetAdminUserByIdQuery(int UserId) : IRequest<AdminUserDetailDto?>;
