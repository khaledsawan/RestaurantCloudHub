namespace RestaurantSystem.Application.Features.Menu.DTOs;

public class MenuItemDetailDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsFeatured { get; set; }
    public int PreparationTimeMinutes { get; set; }
    public int? Calories { get; set; }
    public int SpiceLevel { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsDairyFree { get; set; }
    public bool IsNutFree { get; set; }
    public string? AllergenInfo { get; set; }
    public int MaxQuantityPerOrder { get; set; }

    public List<OptionGroupDto> OptionGroups { get; set; } = new();
}
