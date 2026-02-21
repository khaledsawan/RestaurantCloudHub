using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Orders.EventHandlers;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public CancelOrderCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure("Order not found");
        }

        if (order.OrderStatus == OrderStatus.Completed)
        {
            return Result.Failure("Completed orders cannot be cancelled");
        }

        var isStaff = _currentUserService.IsInAnyRole("Admin", "Manager");
        if (!isStaff)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return Result.Failure("User not authenticated");
            }

            var customerId = await _context.Customers
                .Where(c => c.UserId == _currentUserService.UserId.Value)
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (!customerId.HasValue || order.CustomerId != customerId.Value)
            {
                return Result.Failure("Not authorized to cancel this order");
            }
        }

        var fromStatus = order.OrderStatus;
        order.OrderStatus = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;
        order.CancellationReason = request.Reason;
        order.CancelledByType = isStaff ? CancelledByType.Staff : CancelledByType.Customer;

        var staffId = isStaff
            ? await _context.Staff
                .Where(s => s.UserId == _currentUserService.UserId)
                .Select(s => (int?)s.Id)
                .FirstOrDefaultAsync(cancellationToken)
            : null;

        var changedById = isStaff ? staffId : order.CustomerId;
        var changedByType = isStaff ? "staff" : "customer";

        order.StatusHistory.Add(new Domain.Entities.OrderStatusHistory
        {
            FromStatus = fromStatus,
            ToStatus = OrderStatus.Cancelled,
            ChangedById = changedById,
            ChangedByType = changedByType,
            Notes = request.Reason
        });

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new OrderStatusChangedEvent(order.Id, fromStatus, order.OrderStatus), cancellationToken);

        return Result.Success();
    }
}
