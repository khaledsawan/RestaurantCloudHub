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
        if (request.OrderId.HasValue)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.OrderId == request.OrderId.Value)
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

        if (!_currentUserService.UserId.HasValue)
        {
            return new List<PaymentDto>();
        }

        var customerId = await _context.Customers
            .Where(c => c.UserId == _currentUserService.UserId.Value)
            .Select(c => (int?)c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!customerId.HasValue)
        {
            return new List<PaymentDto>();
        }

        return await _context.Payments
            .AsNoTracking()
            .Join(_context.Orders,
                p => p.OrderId,
                o => o.Id,
                (p, o) => new { Payment = p, Order = o })
            .Where(x => x.Order.CustomerId == customerId.Value)
            .OrderByDescending(x => x.Payment.PaymentDate)
            .Select(x => new PaymentDto
            {
                Id = x.Payment.Id,
                OrderId = x.Payment.OrderId,
                PaymentMethod = x.Payment.PaymentMethod,
                Amount = x.Payment.Amount,
                PaymentStatus = x.Payment.PaymentStatus,
                PaymentDate = x.Payment.PaymentDate
            })
            .ToListAsync(cancellationToken);
    }
}
