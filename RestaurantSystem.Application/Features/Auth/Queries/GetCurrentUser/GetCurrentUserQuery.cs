using MediatR;
using RestaurantSystem.Application.Features.Auth.DTOs;

namespace RestaurantSystem.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<CurrentUserDto>;
