using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Orders.DTOs;
using RestaurantSystem.Application.Features.Orders.EventHandlers;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrderSettings _orderSettings;
    private readonly IMediator _mediator;

    public CreateOrderCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IOrderSettings orderSettings,
        IMediator mediator)
    {
        _context = context;
        _currentUserService = currentUserService;
        _orderSettings = orderSettings;
        _mediator = mediator;
    }

    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Result<OrderDto>.Failure("User not authenticated");
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId.Value, cancellationToken);

        if (customer == null)
        {
            return Result<OrderDto>.Failure("Customer profile not found");
        }

        var orderRequest = request.Order;
        if (orderRequest.Items.Count == 0)
        {
            return Result<OrderDto>.Failure("At least one order item is required");
        }

        var itemIds = orderRequest.Items.Select(i => i.ItemId).Distinct().ToList();
        var menuItems = await _context.MenuItems
            .Where(m => itemIds.Contains(m.Id))
            .ToListAsync(cancellationToken);

        if (menuItems.Count != itemIds.Count)
        {
            return Result<OrderDto>.Failure("One or more menu items not found");
        }

        var optionIds = orderRequest.Items
            .SelectMany(i => i.SelectedOptionIds)
            .Distinct()
            .ToList();

        var options = optionIds.Count == 0
            ? new List<MenuItemOption>()
            : await _context.MenuItemOptions
                .Include(o => o.OptionGroup)
                .Where(o => optionIds.Contains(o.Id))
                .ToListAsync(cancellationToken);

        if (options.Count != optionIds.Count)
        {
            return Result<OrderDto>.Failure("One or more selected options not found");
        }

        var optionsById = options.ToDictionary(o => o.Id, o => o);

        var order = new Order
        {
            OrderNumber = await GenerateOrderNumberAsync(cancellationToken),
            CustomerId = customer.Id,
            OrderType = orderRequest.OrderType,
            OrderStatus = OrderStatus.Pending,
            TaxRate = _orderSettings.TaxRate,
            DeliveryFee = orderRequest.OrderType == OrderType.Delivery ? _orderSettings.DeliveryFee : 0m,
            TipAmount = orderRequest.TipAmount ?? 0m,
            CustomerNotes = orderRequest.CustomerNotes,
            KitchenNotes = orderRequest.KitchenNotes,
            DeliveryNotes = orderRequest.DeliveryNotes,
            DeliveryAddressId = orderRequest.DeliveryAddressId
        };

        decimal subtotal = 0m;

        foreach (var item in orderRequest.Items)
        {
            var menuItem = menuItems.First(m => m.Id == item.ItemId);
            var selectedOptions = item.SelectedOptionIds
                .Select(id => optionsById[id])
                .ToList();

            if (selectedOptions.Any(o => o.OptionGroup.ItemId != menuItem.Id))
            {
                return Result<OrderDto>.Failure("Selected option does not belong to menu item");
            }

            var optionsTotal = selectedOptions.Sum(o => o.PriceAdjustment);
            var unitPrice = menuItem.Price + optionsTotal;
            var lineSubtotal = unitPrice * item.Quantity;

            var orderItem = new OrderItem
            {
                ItemId = menuItem.Id,
                Quantity = item.Quantity,
                UnitPrice = menuItem.Price,
                Subtotal = lineSubtotal,
                ItemNotes = item.ItemNotes,
                ItemStatus = OrderItemStatus.Pending
            };

            foreach (var option in selectedOptions)
            {
                orderItem.SelectedOptions.Add(new OrderItemOption
                {
                    OptionId = option.Id,
                    OptionGroupName = option.OptionGroup.Name,
                    OptionName = option.Name,
                    PriceAdjustment = option.PriceAdjustment
                });
            }

            order.OrderItems.Add(orderItem);
            subtotal += lineSubtotal;
        }

        order.Subtotal = subtotal;
        order.TaxAmount = Math.Round(subtotal * order.TaxRate, 2);
        order.TotalAmount = subtotal + order.TaxAmount + order.DeliveryFee + order.TipAmount - order.DiscountAmount;

        order.StatusHistory.Add(new OrderStatusHistory
        {
            FromStatus = null,
            ToStatus = OrderStatus.Pending,
            ChangedById = customer.Id,
            ChangedByType = "customer",
            Notes = "Order created"
        });

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new OrderCreatedEvent(order.Id, order.OrderNumber, customer.Id), cancellationToken);

        var dto = new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderStatus = order.OrderStatus,
            OrderType = order.OrderType,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt
        };

        return Result<OrderDto>.Success(dto);
    }

    private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
    {
        var prefix = DateTime.UtcNow.ToString("yyyyMMdd");
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var suffix = Random.Shared.Next(0, 9999).ToString("D4");
            var number = $"{prefix}-{suffix}";

            var exists = await _context.Orders.AnyAsync(o => o.OrderNumber == number, cancellationToken);
            if (!exists)
            {
                return number;
            }
        }

        return $"{prefix}-{Guid.NewGuid():N}"[..20];
    }
}
