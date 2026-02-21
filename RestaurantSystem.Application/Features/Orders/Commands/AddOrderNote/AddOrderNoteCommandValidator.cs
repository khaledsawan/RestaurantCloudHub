using FluentValidation;

namespace RestaurantSystem.Application.Features.Orders.Commands.AddOrderNote;

public class AddOrderNoteCommandValidator : AbstractValidator<AddOrderNoteCommand>
{
    public AddOrderNoteCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("OrderId is required");

        RuleFor(x => x.Note)
            .NotEmpty().WithMessage("Note is required")
            .MaximumLength(1000);
    }
}
