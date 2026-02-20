using MediatR;
using RestaurantSystem.Application.Features.Menu.DTOs;

namespace RestaurantSystem.Application.Features.Menu.Queries.GetMenuItems;

public record GetMenuItemsQuery : IRequest<List<MenuItemDto>>
{
    public int? CategoryId { get; init; }
    public bool? AvailableOnly { get; init; }
    public string? Search { get; init; }
}
