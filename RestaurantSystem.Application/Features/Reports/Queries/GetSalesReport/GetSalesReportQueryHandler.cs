using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Reports.DTOs;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Reports.Queries.GetSalesReport;

public class GetSalesReportQueryHandler : IRequestHandler<GetSalesReportQuery, SalesReportDto>
{
    private readonly IApplicationDbContext _context;

    public GetSalesReportQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesReportDto> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Orders.AsNoTracking().Where(o => o.OrderStatus == OrderStatus.Completed);

        if (request.DateFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= request.DateTo.Value);
        }

        var totalOrders = await query.CountAsync(cancellationToken);
        var totalSales = await query.SumAsync(o => (decimal?)o.TotalAmount, cancellationToken) ?? 0m;

        return new SalesReportDto
        {
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            TotalOrders = totalOrders,
            TotalSales = totalSales,
            AverageOrderValue = totalOrders == 0 ? 0 : totalSales / totalOrders
        };
    }
}
