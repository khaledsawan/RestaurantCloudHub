using FluentValidation;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Admin.Commands.UpdateAdminUserRole;

public class UpdateAdminUserRoleCommandValidator : AbstractValidator<UpdateAdminUserRoleCommand>
{
    public UpdateAdminUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => Enum.GetNames<UserRole>().Contains(role, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid role");
    }
}
