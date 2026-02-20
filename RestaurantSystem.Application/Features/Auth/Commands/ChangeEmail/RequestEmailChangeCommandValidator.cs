using FluentValidation;

namespace RestaurantSystem.Application.Features.Auth.Commands.ChangeEmail;

public class RequestEmailChangeCommandValidator : AbstractValidator<RequestEmailChangeCommand>
{
    public RequestEmailChangeCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
