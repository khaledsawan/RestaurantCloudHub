using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using StaffEntity = RestaurantSystem.Domain.Entities.Staff;

namespace RestaurantSystem.Application.Features.Staff.Commands.CreateStaffProfile;

public class CreateStaffProfileCommandHandler : IRequestHandler<CreateStaffProfileCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public CreateStaffProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(CreateStaffProfileCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure("User not found");
        }

        var alreadyStaff = await _context.Staff
            .AnyAsync(s => s.UserId == request.UserId, cancellationToken);

        if (alreadyStaff)
        {
            return Result.Failure("Staff profile already exists");
        }

        _context.Staff.Add(new StaffEntity
        {
            UserId = request.UserId,
            Phone = request.Phone,
            ProfileImageUrl = request.ProfileImageUrl,
            IsActive = true
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
