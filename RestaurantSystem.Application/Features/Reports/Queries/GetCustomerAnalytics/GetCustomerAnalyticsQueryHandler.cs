using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Reports.DTOs;

namespace RestaurantSystem.Application.Features.Reports.Queries.GetCustomerAnalytics;

public class GetCustomerAnalyticsQueryHandler : IRequestHandler<GetCustomerAnalyticsQuery, AnalyticsDto>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerAnalyticsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsDto> Handle(GetCustomerAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var totalCustomers = await _context.Customers.CountAsync(cancellationToken);
        var activeCustomers = await _context.Customers.CountAsync(c => c.IsActive, cancellationToken);
        var totalOrders = await _context.Orders.CountAsync(cancellationToken);
        var totalSales = await _context.Orders.SumAsync(o => (decimal?)o.TotalAmount, cancellationToken) ?? 0m;

        return new AnalyticsDto
        {
            TotalCustomers = totalCustomers,
            ActiveCustomers = activeCustomers,
            TotalOrders = totalOrders,
            AverageOrderValue = totalOrders == 0 ? 0 : totalSales / totalOrders
        };
    }
}
