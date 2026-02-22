using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Payments.DTOs;

namespace RestaurantSystem.Application.Features.Payments.Queries.GetPaymentHistory;

public class GetPaymentHistoryQueryHandler : IRequestHandler<GetPaymentHistoryQuery, List<PaymentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetPaymentHistoryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<PaymentDto>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        var paymentsQuery = _context.Payments.AsNoTracking().AsQueryable();

        if (request.OrderId.HasValue)
        {
            paymentsQuery = paymentsQuery.Where(p => p.OrderId == request.OrderId.Value);
        }
        else if (_currentUserService.UserId.HasValue)
        {
            var customerId = await _context.Customers
                .Where(c => c.UserId == _currentUserService.UserId.Value)
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (customerId.HasValue)
            {
                paymentsQuery = paymentsQuery
                    .Where(p => _context.Orders.Any(o => o.Id == p.OrderId && o.CustomerId == customerId.Value));
            }
            else
            {
                return new List<PaymentDto>();
            }
        }

        return await paymentsQuery
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                PaymentStatus = p.PaymentStatus,
                PaymentDate = p.PaymentDate
            })
            .ToListAsync(cancellationToken);
    }
}
