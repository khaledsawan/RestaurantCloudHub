using FluentValidation;

namespace RestaurantSystem.Application.Features.Menu.Commands.CreateMenuItem;

public class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    public CreateMenuItemCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PreparationTimeMinutes)
            .GreaterThan(0);

        RuleFor(x => x.SpiceLevel)
            .InclusiveBetween(0, 5);

        RuleFor(x => x.MaxQuantityPerOrder)
            .GreaterThan(0);

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500);

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(500);
    }
}
