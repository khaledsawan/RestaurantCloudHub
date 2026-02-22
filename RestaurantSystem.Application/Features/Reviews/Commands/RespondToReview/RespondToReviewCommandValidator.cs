using FluentValidation;

namespace RestaurantSystem.Application.Features.Reviews.Commands.RespondToReview;

public class RespondToReviewCommandValidator : AbstractValidator<RespondToReviewCommand>
{
    public RespondToReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .GreaterThan(0).WithMessage("ReviewId is required");

        RuleFor(x => x.ResponseText)
            .NotEmpty().WithMessage("Response text is required")
            .MaximumLength(2000);
    }
}
