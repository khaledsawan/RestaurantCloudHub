using MediatR;
using RestaurantSystem.Application.Features.Orders.DTOs;

namespace RestaurantSystem.Application.Features.Orders.Queries.GetActiveOrders;

public record GetActiveOrdersQuery : IRequest<List<OrderSummaryDto>>;
