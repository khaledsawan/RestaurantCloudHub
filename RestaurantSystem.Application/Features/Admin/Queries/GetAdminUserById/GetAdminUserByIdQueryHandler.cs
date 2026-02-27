using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Admin.DTOs;

namespace RestaurantSystem.Application.Features.Admin.Queries.GetAdminUserById;

public class GetAdminUserByIdQueryHandler : IRequestHandler<GetAdminUserByIdQuery, AdminUserDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAdminUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminUserDetailDto?> Handle(GetAdminUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return null;
        }

        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);

        var staff = await _context.Staff
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == user.Id, cancellationToken);

        return new AdminUserDetailDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            Roles = user.UserRoles.Select(r => r.RoleName),
            CreatedAt = user.CreatedAt,
            CustomerId = customer?.Id,
            CustomerIsActive = customer?.IsActive,
            CustomerDeletedAt = customer?.DeletedAt,
            StaffId = staff?.Id,
            StaffIsActive = staff?.IsActive,
            StaffDeletedAt = staff?.DeletedAt
        };
    }
}
