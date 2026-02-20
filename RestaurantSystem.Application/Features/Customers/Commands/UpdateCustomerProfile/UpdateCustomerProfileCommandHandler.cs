using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Customers.Commands.UpdateCustomerProfile;

public class UpdateCustomerProfileCommandHandler : IRequestHandler<UpdateCustomerProfileCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCustomerProfileCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue || string.IsNullOrWhiteSpace(_currentUserService.Email))
        {
            return Result.Failure("User not authenticated");
        }

        var userId = _currentUserService.UserId.Value;

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (customer == null)
        {
            return Result.Failure("Customer profile not found");
        }

        customer.Phone = request.Phone;
        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.DateOfBirth = request.DateOfBirth;
        customer.ProfileImageUrl = request.ProfileImageUrl;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
