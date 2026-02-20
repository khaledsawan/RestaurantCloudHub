using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Menu.DTOs;

public class OptionGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsRequired { get; set; }
    public OptionSelectionType SelectionType { get; set; }
    public int MinSelections { get; set; }
    public int MaxSelections { get; set; }
    public int DisplayOrder { get; set; }

    public List<OptionDto> Options { get; set; } = new();
}
