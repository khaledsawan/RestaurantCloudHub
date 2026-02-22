namespace RestaurantSystem.Application.Features.Reservations.DTOs;

public class TableAvailabilityDto
{
    public int TableId { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }
}
