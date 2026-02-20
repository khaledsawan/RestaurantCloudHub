using FluentValidation;

namespace RestaurantSystem.Application.Features.Customers.Commands.CreateCustomerProfile;

public class CreateCustomerProfileCommandValidator : AbstractValidator<CreateCustomerProfileCommand>
{
    public CreateCustomerProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .MaximumLength(20);
    }
}
