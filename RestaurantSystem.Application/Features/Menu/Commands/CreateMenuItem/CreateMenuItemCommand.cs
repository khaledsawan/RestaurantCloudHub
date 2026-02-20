using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.CreateMenuItem;

public record CreateMenuItemCommand : IRequest<Result>
{
    public int CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string? ImageUrl { get; init; }
    public string? ThumbnailUrl { get; init; }
    public bool IsAvailable { get; init; } = true;
    public bool IsFeatured { get; init; } = false;
    public int PreparationTimeMinutes { get; init; } = 15;
    public int? Calories { get; init; }
    public int SpiceLevel { get; init; } = 0;
    public bool IsVegetarian { get; init; } = false;
    public bool IsVegan { get; init; } = false;
    public bool IsGlutenFree { get; init; } = false;
    public bool IsDairyFree { get; init; } = false;
    public bool IsNutFree { get; init; } = false;
    public string? AllergenInfo { get; init; }
    public int MaxQuantityPerOrder { get; init; } = 10;
}
