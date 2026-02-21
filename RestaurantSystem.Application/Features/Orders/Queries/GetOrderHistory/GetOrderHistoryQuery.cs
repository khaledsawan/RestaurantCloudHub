using MediatR;
using RestaurantSystem.Application.Features.Orders.DTOs;

namespace RestaurantSystem.Application.Features.Orders.Queries.GetOrderHistory;

public record GetOrderHistoryQuery : IRequest<List<OrderSummaryDto>>
{
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}
