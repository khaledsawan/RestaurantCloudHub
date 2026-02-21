using MediatR;
using RestaurantSystem.Application.Features.Orders.DTOs;

namespace RestaurantSystem.Application.Features.Orders.Queries.GetMyOrders;

public record GetMyOrdersQuery : IRequest<List<OrderSummaryDto>>;
