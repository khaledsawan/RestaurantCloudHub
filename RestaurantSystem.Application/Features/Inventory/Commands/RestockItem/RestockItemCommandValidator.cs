using FluentValidation;

namespace RestaurantSystem.Application.Features.Inventory.Commands.RestockItem;

public class RestockItemCommandValidator : AbstractValidator<RestockItemCommand>
{
    public RestockItemCommandValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .GreaterThan(0).WithMessage("InventoryItemId is required");

        RuleFor(x => x.QuantityAdded)
            .GreaterThan(0).WithMessage("QuantityAdded must be greater than 0");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0)
            .When(x => x.UnitCost.HasValue);
    }
}
