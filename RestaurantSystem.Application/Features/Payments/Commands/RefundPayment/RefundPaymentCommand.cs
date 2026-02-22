using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Payments.Commands.RefundPayment;

public record RefundPaymentCommand : IRequest<Result>
{
    public int PaymentId { get; init; }
    public decimal? RefundAmount { get; init; }
    public string? Reason { get; init; }
}
