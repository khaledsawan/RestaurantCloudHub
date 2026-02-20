using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Features.Customers.Commands.CreateCustomerProfile;

public class CreateCustomerProfileCommandHandler : IRequestHandler<CreateCustomerProfileCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateCustomerProfileCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CreateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue || string.IsNullOrWhiteSpace(_currentUserService.Email))
        {
            return Result.Failure("User not authenticated");
        }

        var userId = _currentUserService.UserId.Value;
        var email = _currentUserService.Email!;

        var exists = await _context.Customers.AnyAsync(c => c.UserId == userId, cancellationToken);
        if (exists)
        {
            return Result.Failure("Customer profile already exists");
        }

        var emailUsed = await _context.Customers.AnyAsync(c => c.Email == email, cancellationToken);
        if (emailUsed)
        {
            return Result.Failure("Email already in use");
        }

        var customer = new Customer
        {
            UserId = userId,
            Email = email,
            Phone = request.Phone,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            ProfileImageUrl = request.ProfileImageUrl,
            IsActive = true,
            IsVerified = false
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
