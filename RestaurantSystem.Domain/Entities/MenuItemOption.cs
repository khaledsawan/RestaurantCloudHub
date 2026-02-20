using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities;

public class MenuItemOption : BaseEntity
{
    public int OptionGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; } = 0;
    public int CaloriesAdjustment { get; set; } = 0;
    public bool IsAvailable { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;

    public virtual MenuItemOptionGroup OptionGroup { get; set; } = null!;
}
