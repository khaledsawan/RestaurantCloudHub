using MediatR;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Orders.DTOs;

namespace RestaurantSystem.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(CreateOrderDto Order) : IRequest<Result<OrderDto>>;
