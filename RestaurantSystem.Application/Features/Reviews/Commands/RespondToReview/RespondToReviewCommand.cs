using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Reviews.Commands.RespondToReview;

public record RespondToReviewCommand : IRequest<Result>
{
    public int ReviewId { get; init; }
    public string ResponseText { get; init; } = string.Empty;
}
