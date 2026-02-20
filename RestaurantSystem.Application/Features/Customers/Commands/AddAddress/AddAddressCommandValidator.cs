using FluentValidation;

namespace RestaurantSystem.Application.Features.Customers.Commands.AddAddress;

public class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("AddressLine1 is required")
            .MaximumLength(255);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(255);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100);

        RuleFor(x => x.State)
            .MaximumLength(50);

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("PostalCode is required")
            .MaximumLength(20);

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(100);
    }
}
