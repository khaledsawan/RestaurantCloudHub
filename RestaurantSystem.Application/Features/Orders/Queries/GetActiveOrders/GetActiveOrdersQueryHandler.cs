using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Orders.DTOs;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.Queries.GetActiveOrders;

public class GetActiveOrdersQueryHandler : IRequestHandler<GetActiveOrdersQuery, List<OrderSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetActiveOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderSummaryDto>> Handle(GetActiveOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.OrderStatus != OrderStatus.Completed && o.OrderStatus != OrderStatus.Cancelled)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderStatus = o.OrderStatus,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
