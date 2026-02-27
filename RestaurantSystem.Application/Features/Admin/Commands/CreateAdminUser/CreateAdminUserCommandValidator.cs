using FluentValidation;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Admin.Commands.CreateAdminUser;

public class CreateAdminUserCommandValidator : AbstractValidator<CreateAdminUserCommand>
{
    public CreateAdminUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => Enum.GetNames<UserRole>().Contains(role, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid role");
    }
}
