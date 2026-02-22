using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand : IRequest<Result>
{
    public int OrderId { get; init; }
    public int Rating { get; init; }
    public int? FoodRating { get; init; }
    public int? ServiceRating { get; init; }
    public int? DeliveryRating { get; init; }
    public string? ReviewText { get; init; }
}
