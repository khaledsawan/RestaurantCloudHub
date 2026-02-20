using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Customers.DTOs;

namespace RestaurantSystem.Application.Features.Customers.Queries.GetLoyaltyPoints;

public class GetLoyaltyPointsQueryHandler : IRequestHandler<GetLoyaltyPointsQuery, LoyaltyPointsDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetLoyaltyPointsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<LoyaltyPointsDto?> Handle(GetLoyaltyPointsQuery request, CancellationToken cancellationToken)
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

        return new LoyaltyPointsDto
        {
            LoyaltyPoints = customer.LoyaltyPoints,
            TotalOrders = customer.TotalOrders,
            TotalSpent = customer.TotalSpent
        };
    }
}
