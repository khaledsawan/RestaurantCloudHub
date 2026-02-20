using MediatR;
using RestaurantSystem.Application.Features.Menu.DTOs;

namespace RestaurantSystem.Application.Features.Menu.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>
{
    public bool? ActiveOnly { get; init; }
}
