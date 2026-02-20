namespace RestaurantSystem.Application.Features.Menu.DTOs;

public class OptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceAdjustment { get; set; }
    public int CaloriesAdjustment { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsDefault { get; set; }
    public int DisplayOrder { get; set; }
}
