using FluentValidation;

namespace RestaurantSystem.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Order)
            .NotNull().WithMessage("Order is required");

        RuleFor(x => x.Order.Items)
            .NotNull()
            .Must(items => items.Count > 0)
            .WithMessage("At least one order item is required");

        RuleForEach(x => x.Order.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ItemId)
                    .GreaterThan(0).WithMessage("ItemId is required");
                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0");
            });
    }
}
