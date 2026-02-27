using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Admin.DTOs;

namespace RestaurantSystem.Application.Features.Admin.Commands.CreateAdminUser;

public class CreateAdminUserCommandHandler : IRequestHandler<CreateAdminUserCommand, Result<AdminUserDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public CreateAdminUserCommandHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async Task<Result<AdminUserDto>> Handle(CreateAdminUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedRole = NormalizeRole(request.Role);
        var result = await _identityService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            normalizedRole);

        if (!result.Succeeded)
        {
            return Result<AdminUserDto>.Failure(result.Errors);
        }

        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .FirstAsync(u => u.Id == result.Data, cancellationToken);

        var dto = new AdminUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            EmailConfirmed = user.EmailConfirmed,
            Roles = user.UserRoles.Select(r => r.RoleName),
            CreatedAt = user.CreatedAt
        };

        return Result<AdminUserDto>.Success(dto);
    }

    private static string NormalizeRole(string role)
    {
        return Enum.TryParse(role, true, out RestaurantSystem.Domain.Enums.UserRole parsed)
            ? parsed.ToString()
            : role;
    }
}
