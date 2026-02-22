using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class RestaurantTable : BaseEntity
{
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public TableStatus Status { get; set; } = TableStatus.Available;
    public string? Location { get; set; }
    public string? QrCodeUrl { get; set; }
}
