using FluentValidation;

namespace RestaurantSystem.Application.Features.Menu.Commands.AddOption;

public class AddOptionCommandValidator : AbstractValidator<AddOptionCommand>
{
    public AddOptionCommandValidator()
    {
        RuleFor(x => x.OptionGroupId)
            .GreaterThan(0).WithMessage("OptionGroupId is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);

        RuleFor(x => x.PriceAdjustment)
            .GreaterThanOrEqualTo(-10000);
    }
}
