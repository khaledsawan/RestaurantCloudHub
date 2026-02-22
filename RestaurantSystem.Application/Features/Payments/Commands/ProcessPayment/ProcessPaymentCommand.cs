using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Payments.Commands.ProcessPayment;

public record ProcessPaymentCommand : MediatR.IRequest<RestaurantSystem.Application.Common.Models.Result<RestaurantSystem.Application.Features.Payments.DTOs.PaymentResponseDto>>
{
    public int OrderId { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public decimal Amount { get; init; }
}
