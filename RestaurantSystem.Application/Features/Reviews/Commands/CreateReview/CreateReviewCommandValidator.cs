using FluentValidation;

namespace RestaurantSystem.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("OrderId is required");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.FoodRating)
            .InclusiveBetween(1, 5)
            .When(x => x.FoodRating.HasValue);

        RuleFor(x => x.ServiceRating)
            .InclusiveBetween(1, 5)
            .When(x => x.ServiceRating.HasValue);

        RuleFor(x => x.DeliveryRating)
            .InclusiveBetween(1, 5)
            .When(x => x.DeliveryRating.HasValue);
    }
}
