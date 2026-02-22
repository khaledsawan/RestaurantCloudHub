namespace RestaurantSystem.Application.Features.Reports.DTOs;

public class PopularItemDto
{
    public int ItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
}
