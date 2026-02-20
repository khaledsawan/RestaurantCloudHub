using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Customers.DTOs;

namespace RestaurantSystem.Application.Features.Customers.Queries.GetCustomerProfile;

public class GetCustomerProfileQueryHandler : IRequestHandler<GetCustomerProfileQuery, CustomerProfileDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCustomerProfileQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CustomerProfileDto?> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return null;
        }

        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId.Value, cancellationToken);

        if (customer == null)
        {
            return null;
        }

        return new CustomerProfileDto
        {
            Email = customer.Email,
            Phone = customer.Phone,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            DateOfBirth = customer.DateOfBirth,
            ProfileImageUrl = customer.ProfileImageUrl,
            IsVerified = customer.IsVerified,
            LoyaltyPoints = customer.LoyaltyPoints,
            TotalOrders = customer.TotalOrders,
            TotalSpent = customer.TotalSpent,
            AverageRating = customer.AverageRating,
            CreatedAt = customer.CreatedAt
        };
    }
}
