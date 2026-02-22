using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Payments.Commands.RefundPayment;

public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public RefundPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);

        if (payment == null)
        {
            return Result.Failure("Payment not found");
        }

        if (payment.PaymentStatus == PaymentStatus.Refunded)
        {
            return Result.Failure("Payment already refunded");
        }

        var refundAmount = request.RefundAmount ?? payment.Amount;
        if (refundAmount <= 0 || refundAmount > payment.Amount)
        {
            return Result.Failure("Invalid refund amount");
        }

        payment.RefundAmount = refundAmount;
        payment.RefundReason = request.Reason;
        payment.RefundedAt = DateTime.UtcNow;
        payment.PaymentStatus = refundAmount < payment.Amount
            ? PaymentStatus.PartiallyRefunded
            : PaymentStatus.Refunded;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
