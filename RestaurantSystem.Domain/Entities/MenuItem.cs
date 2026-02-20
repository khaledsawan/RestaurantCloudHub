using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities;

public class MenuItem : BaseAuditableEntity, ISoftDeletable
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public int PreparationTimeMinutes { get; set; } = 15;
    public int? Calories { get; set; }
    public int SpiceLevel { get; set; } = 0;
    public bool IsVegetarian { get; set; } = false;
    public bool IsVegan { get; set; } = false;
    public bool IsGlutenFree { get; set; } = false;
    public bool IsDairyFree { get; set; } = false;
    public bool IsNutFree { get; set; } = false;
    public string? AllergenInfo { get; set; }
    public int MaxQuantityPerOrder { get; set; } = 10;
    public DateTime? DeletedAt { get; set; }

    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<MenuItemOptionGroup> OptionGroups { get; set; } = new List<MenuItemOptionGroup>();
}
