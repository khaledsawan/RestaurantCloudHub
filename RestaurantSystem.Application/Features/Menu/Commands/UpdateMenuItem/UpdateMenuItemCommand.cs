using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.UpdateMenuItem;

public record UpdateMenuItemCommand : IRequest<Result>
{
    public int Id { get; init; }
    public int? CategoryId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public string? ImageUrl { get; init; }
    public string? ThumbnailUrl { get; init; }
    public bool? IsAvailable { get; init; }
    public bool? IsFeatured { get; init; }
    public int? PreparationTimeMinutes { get; init; }
    public int? Calories { get; init; }
    public int? SpiceLevel { get; init; }
    public bool? IsVegetarian { get; init; }
    public bool? IsVegan { get; init; }
    public bool? IsGlutenFree { get; init; }
    public bool? IsDairyFree { get; init; }
    public bool? IsNutFree { get; init; }
    public string? AllergenInfo { get; init; }
    public int? MaxQuantityPerOrder { get; init; }
}
