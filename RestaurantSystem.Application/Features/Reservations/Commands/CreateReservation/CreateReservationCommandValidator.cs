using FluentValidation;

namespace RestaurantSystem.Application.Features.Reservations.Commands.CreateReservation;

public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.Reservation).NotNull();

        RuleFor(x => x.Reservation.TableId)
            .GreaterThan(0).WithMessage("TableId is required");

        RuleFor(x => x.Reservation.PartySize)
            .GreaterThan(0).WithMessage("Party size must be greater than 0");
    }
}
