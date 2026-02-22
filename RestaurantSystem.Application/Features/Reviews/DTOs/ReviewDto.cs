namespace RestaurantSystem.Application.Features.Reviews.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
    public string? ResponseText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
