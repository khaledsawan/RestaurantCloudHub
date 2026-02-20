using MediatR;
using RestaurantSystem.Application.Features.Menu.DTOs;

namespace RestaurantSystem.Application.Features.Menu.Queries.GetFeaturedItems;

public record GetFeaturedItemsQuery : IRequest<List<MenuItemDto>>;
