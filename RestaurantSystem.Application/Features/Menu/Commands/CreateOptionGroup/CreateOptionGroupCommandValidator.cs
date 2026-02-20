using FluentValidation;

namespace RestaurantSystem.Application.Features.Menu.Commands.CreateOptionGroup;

public class CreateOptionGroupCommandValidator : AbstractValidator<CreateOptionGroupCommand>
{
    public CreateOptionGroupCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("ItemId is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);

        RuleFor(x => x.SelectionType)
            .IsInEnum()
            .WithMessage("SelectionType must be Single or Multiple");

        RuleFor(x => x.MaxSelections)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.MinSelections)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.MaxSelections);
    }
}
