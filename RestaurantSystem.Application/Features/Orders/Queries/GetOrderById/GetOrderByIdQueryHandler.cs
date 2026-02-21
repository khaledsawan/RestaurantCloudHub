using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Orders.DTOs;

namespace RestaurantSystem.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetOrderByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<OrderDetailDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.MenuItem)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.SelectedOptions)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
        {
            return null;
        }

        var isStaff = _currentUserService.IsInAnyRole("Admin", "Manager");
        if (!isStaff)
        {
            if (!_currentUserService.UserId.HasValue)
            {
                return null;
            }

            var customerId = await _context.Customers
                .Where(c => c.UserId == _currentUserService.UserId.Value)
                .Select(c => (int?)c.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (!customerId.HasValue || order.CustomerId != customerId.Value)
            {
                return null;
            }
        }

        return new OrderDetailDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderStatus = order.OrderStatus,
            OrderType = order.OrderType,
            Subtotal = order.Subtotal,
            TaxRate = order.TaxRate,
            TaxAmount = order.TaxAmount,
            DeliveryFee = order.DeliveryFee,
            DiscountAmount = order.DiscountAmount,
            TipAmount = order.TipAmount,
            TotalAmount = order.TotalAmount,
            CustomerNotes = order.CustomerNotes,
            KitchenNotes = order.KitchenNotes,
            DeliveryNotes = order.DeliveryNotes,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.OrderItems.Select(i => new OrderItemDto
            {
                ItemId = i.ItemId,
                Name = i.MenuItem.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Subtotal,
                ItemNotes = i.ItemNotes,
                Options = i.SelectedOptions.Select(o => new OrderItemOptionDto
                {
                    OptionId = o.OptionId,
                    OptionGroupName = o.OptionGroupName,
                    OptionName = o.OptionName,
                    PriceAdjustment = o.PriceAdjustment
                }).ToList()
            }).ToList()
        };
    }
}
