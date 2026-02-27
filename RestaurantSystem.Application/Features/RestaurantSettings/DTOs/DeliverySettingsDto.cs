namespace RestaurantSystem.Application.Features.RestaurantSettings.DTOs;

public class DeliverySettingsDto
{
    public bool Enabled { get; set; } = true;
    public decimal RadiusKm { get; set; } = 5;
    public decimal MinOrder { get; set; }
    public decimal Fee { get; set; }
    public int EstimatedMinutes { get; set; } = 45;
}
