using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Admin.DTOs;

namespace RestaurantSystem.Application.Features.Admin.Queries.GetAdminUsers;

public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, PaginatedList<AdminUserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAdminUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AdminUserDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var role = request.Role.ToLowerInvariant();
            query = query.Where(u => u.UserRoles.Any(r => r.RoleName.ToLower() == role));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        if (request.EmailConfirmed.HasValue)
        {
            query = query.Where(u => u.EmailConfirmed == request.EmailConfirmed.Value);
        }

        if (request.CreatedFrom.HasValue)
        {
            query = query.Where(u => u.CreatedAt >= request.CreatedFrom.Value);
        }

        if (request.CreatedTo.HasValue)
        {
            query = query.Where(u => u.CreatedAt <= request.CreatedTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(u =>
                EF.Functions.Like(u.Email.ToLower(), $"%{search}%") ||
                EF.Functions.Like(u.FirstName.ToLower(), $"%{search}%") ||
                EF.Functions.Like(u.LastName.ToLower(), $"%{search}%"));
        }

        query = query.OrderByDescending(u => u.CreatedAt);

        var projected = query.Select(u => new AdminUserDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsActive = u.IsActive,
            EmailConfirmed = u.EmailConfirmed,
            Roles = u.UserRoles.Select(r => r.RoleName),
            CreatedAt = u.CreatedAt
        });

        return await PaginatedList<AdminUserDto>.CreateAsync(projected, request.PageNumber, request.PageSize, cancellationToken);
    }
}
