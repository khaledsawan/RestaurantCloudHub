using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities;

public class CustomerAddress : BaseEntity
{
    public int CustomerId { get; set; }
    public string? Label { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string? State { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = "USA";
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? DeliveryInstructions { get; set; }
    public bool IsDefault { get; set; } = false;

    public virtual Customer Customer { get; set; } = null!;
}
