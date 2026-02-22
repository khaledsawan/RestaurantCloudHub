using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Payments.DTOs;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<PaymentResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public ProcessPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaymentResponseDto>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result<PaymentResponseDto>.Failure("Order not found");
        }

        if (request.Amount <= 0)
        {
            return Result<PaymentResponseDto>.Failure("Invalid amount");
        }

        var payment = new Payment
        {
            OrderId = order.Id,
            PaymentMethod = request.PaymentMethod,
            Amount = request.Amount,
            PaymentStatus = PaymentStatus.Completed,
            PaymentDate = DateTime.UtcNow
        };

        _context.Payments.Add(payment);

        if (order.OrderStatus == OrderStatus.Pending)
        {
            order.OrderStatus = OrderStatus.Confirmed;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<PaymentResponseDto>.Success(new PaymentResponseDto
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            Status = payment.PaymentStatus,
            Amount = payment.Amount
        });
    }
}
