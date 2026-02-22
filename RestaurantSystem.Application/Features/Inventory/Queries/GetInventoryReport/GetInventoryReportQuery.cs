using MediatR;
using RestaurantSystem.Application.Features.Inventory.DTOs;

namespace RestaurantSystem.Application.Features.Inventory.Queries.GetInventoryReport;

public record GetInventoryReportQuery : IRequest<List<InventoryTransactionDto>>
{
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}
