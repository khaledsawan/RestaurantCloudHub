using MediatR;
using RestaurantSystem.Application.Features.Auth.DTOs;

namespace RestaurantSystem.Application.Features.Auth.Queries.ValidateToken;

public record ValidateTokenQuery(string Token) : IRequest<TokenValidationDto>;
