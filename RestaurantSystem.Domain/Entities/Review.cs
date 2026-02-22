using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities;

public class Review : BaseEntity
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int Rating { get; set; }
    public int? FoodRating { get; set; }
    public int? ServiceRating { get; set; }
    public int? DeliveryRating { get; set; }
    public string? ReviewText { get; set; }
    public string? ResponseText { get; set; }
    public int? RespondedById { get; set; }
    public DateTime? RespondedAt { get; set; }
    public bool IsPublished { get; set; } = true;

    public virtual Order Order { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual Staff? RespondedBy { get; set; }
}
