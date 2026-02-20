using MediatR;
using RestaurantSystem.Application.Features.Menu.DTOs;

namespace RestaurantSystem.Application.Features.Menu.Queries.GetMenuItemById;

public record GetMenuItemByIdQuery(int Id) : IRequest<MenuItemDetailDto?>;
