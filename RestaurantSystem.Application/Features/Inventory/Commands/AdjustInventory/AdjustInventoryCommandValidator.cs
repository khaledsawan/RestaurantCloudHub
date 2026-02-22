using FluentValidation;

namespace RestaurantSystem.Application.Features.Inventory.Commands.AdjustInventory;

public class AdjustInventoryCommandValidator : AbstractValidator<AdjustInventoryCommand>
{
    public AdjustInventoryCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .GreaterThan(0).WithMessage("InventoryItemId is required");

        RuleFor(x => x.QuantityChange)
            .NotEqual(0).WithMessage("Quantity change must not be zero");
    }
}
