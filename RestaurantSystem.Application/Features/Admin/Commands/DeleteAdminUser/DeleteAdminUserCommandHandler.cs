using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Admin.Commands.DeleteAdminUser;

public class DeleteAdminUserCommandHandler : IRequestHandler<DeleteAdminUserCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;

    public DeleteAdminUserCommandHandler(IApplicationDbContext context, IDateTime dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(DeleteAdminUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        user.IsActive = false;

        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = _dateTime.UtcNow;
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);

        if (customer != null)
        {
            customer.IsActive = false;
            customer.DeletedAt = _dateTime.UtcNow;
        }

        var staff = await _context.Staff
            .FirstOrDefaultAsync(s => s.UserId == user.Id, cancellationToken);

        if (staff != null)
        {
            staff.IsActive = false;
            staff.DeletedAt = _dateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
