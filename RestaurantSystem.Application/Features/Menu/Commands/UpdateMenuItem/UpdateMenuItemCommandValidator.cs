using FluentValidation;

namespace RestaurantSystem.Application.Features.Menu.Commands.UpdateMenuItem;

public class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
{
    public UpdateMenuItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id is required");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => x.Name != null);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Price.HasValue);

        RuleFor(x => x.PreparationTimeMinutes)
            .GreaterThan(0)
            .When(x => x.PreparationTimeMinutes.HasValue);

        RuleFor(x => x.SpiceLevel)
            .InclusiveBetween(0, 5)
            .When(x => x.SpiceLevel.HasValue);

        RuleFor(x => x.MaxQuantityPerOrder)
            .GreaterThan(0)
            .When(x => x.MaxQuantityPerOrder.HasValue);

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500)
            .When(x => x.ImageUrl != null);

        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(500)
            .When(x => x.ThumbnailUrl != null);
    }
}
