using MediatR;
using RestaurantSystem.Application.Features.Payments.DTOs;

namespace RestaurantSystem.Application.Features.Payments.Queries.GetPaymentHistory;

public record GetPaymentHistoryQuery : IRequest<List<PaymentDto>>
{
    public int? OrderId { get; init; }
}
