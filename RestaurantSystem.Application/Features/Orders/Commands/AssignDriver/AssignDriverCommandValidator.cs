using FluentValidation;

namespace RestaurantSystem.Application.Features.Orders.Commands.AssignDriver;

public class AssignDriverCommandValidator : AbstractValidator<AssignDriverCommand>
{
    public AssignDriverCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("OrderId is required");

        RuleFor(x => x.StaffId)
            .GreaterThan(0).WithMessage("StaffId is required");
    }
}
