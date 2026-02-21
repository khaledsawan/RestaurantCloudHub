using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Orders.EventHandlers;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public UpdateOrderStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure("Order not found");
        }

        if (!IsValidTransition(order.OrderStatus, request.Status, order.OrderType))
        {
            return Result.Failure("Invalid status transition");
        }

        var fromStatus = order.OrderStatus;
        order.OrderStatus = request.Status;

        if (request.Status == OrderStatus.Ready)
        {
            order.ActualReadyTime = DateTime.UtcNow;
        }

        if (request.Status == OrderStatus.Completed && order.OrderType == OrderType.Delivery)
        {
            order.ActualDeliveryTime = DateTime.UtcNow;
        }

        var staffId = await _context.Staff
            .Where(s => s.UserId == _currentUserService.UserId)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        order.StatusHistory.Add(new Domain.Entities.OrderStatusHistory
        {
            FromStatus = fromStatus,
            ToStatus = request.Status,
            ChangedById = staffId,
            ChangedByType = "staff",
            Notes = request.Notes
        });

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new OrderStatusChangedEvent(order.Id, fromStatus, order.OrderStatus), cancellationToken);

        if (request.Status == OrderStatus.Completed)
        {
            await _mediator.Publish(new OrderCompletedEvent(order.Id, order.OrderNumber), cancellationToken);
        }

        return Result.Success();
    }

    private static bool IsValidTransition(OrderStatus current, OrderStatus next, OrderType type)
    {
        if (current == OrderStatus.Cancelled || current == OrderStatus.Completed)
        {
            return false;
        }

        return current switch
        {
            OrderStatus.Pending => next is OrderStatus.Confirmed or OrderStatus.Cancelled,
            OrderStatus.Confirmed => next is OrderStatus.Preparing or OrderStatus.Cancelled,
            OrderStatus.Preparing => next is OrderStatus.Ready or OrderStatus.Cancelled,
            OrderStatus.Ready => next is OrderStatus.Completed or OrderStatus.Cancelled or OrderStatus.OutForDelivery,
            OrderStatus.OutForDelivery => next is OrderStatus.Completed or OrderStatus.Cancelled,
            _ => false
        } && (next != OrderStatus.OutForDelivery || type == OrderType.Delivery);
    }
}
