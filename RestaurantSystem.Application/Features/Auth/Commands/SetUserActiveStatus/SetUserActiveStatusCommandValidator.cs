using FluentValidation;

namespace RestaurantSystem.Application.Features.Auth.Commands.SetUserActiveStatus;

public class SetUserActiveStatusCommandValidator : AbstractValidator<SetUserActiveStatusCommand>
{
    public SetUserActiveStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);
    }
}
