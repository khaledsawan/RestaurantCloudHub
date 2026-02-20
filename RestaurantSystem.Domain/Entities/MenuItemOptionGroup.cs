using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class MenuItemOptionGroup : BaseEntity
{
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsRequired { get; set; } = false;
    public OptionSelectionType SelectionType { get; set; } = OptionSelectionType.Single;
    public int MinSelections { get; set; } = 0;
    public int MaxSelections { get; set; } = 1;
    public int DisplayOrder { get; set; } = 0;

    public virtual MenuItem MenuItem { get; set; } = null!;
    public virtual ICollection<MenuItemOption> Options { get; set; } = new List<MenuItemOption>();
}
