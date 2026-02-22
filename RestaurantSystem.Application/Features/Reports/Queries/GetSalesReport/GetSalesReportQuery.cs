using MediatR;
using RestaurantSystem.Application.Features.Reports.DTOs;

namespace RestaurantSystem.Application.Features.Reports.Queries.GetSalesReport;

public record GetSalesReportQuery : IRequest<SalesReportDto>
{
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}
