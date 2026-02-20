using FluentValidation;

namespace RestaurantSystem.Application.Features.Customers.Commands.DeleteAddress;

public class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
{
    public DeleteAddressCommandValidator()
    {
        RuleFor(x => x.AddressId)
            .GreaterThan(0);
    }
}
