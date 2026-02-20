using MediatR;
using RestaurantSystem.Application.Features.Customers.DTOs;

namespace RestaurantSystem.Application.Features.Customers.Queries.GetLoyaltyPoints;

public record GetLoyaltyPointsQuery : IRequest<LoyaltyPointsDto?>;
