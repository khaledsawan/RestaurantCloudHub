using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities.Identity;

namespace RestaurantSystem.Application.Features.Admin.Commands.UpdateAdminUserRole;

public class UpdateAdminUserRoleCommandHandler : IRequestHandler<UpdateAdminUserRoleCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateAdminUserRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateAdminUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        var existingRoles = user.UserRoles.ToList();
        if (existingRoles.Count > 0)
        {
            _context.UserRoles.RemoveRange(existingRoles);
        }

        var normalizedRole = NormalizeRole(request.Role);
        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleName = normalizedRole
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static string NormalizeRole(string role)
    {
        return Enum.TryParse(role, true, out RestaurantSystem.Domain.Enums.UserRole parsed)
            ? parsed.ToString()
            : role;
    }
}
