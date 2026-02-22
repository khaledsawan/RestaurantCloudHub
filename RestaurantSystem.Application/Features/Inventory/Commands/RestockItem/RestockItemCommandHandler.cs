using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Inventory.Commands.RestockItem;

public class RestockItemCommandHandler : IRequestHandler<RestockItemCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public RestockItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(RestockItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == request.InventoryItemId, cancellationToken);

        if (item == null)
        {
            return Result.Failure("Inventory item not found");
        }

        if (request.QuantityAdded <= 0)
        {
            return Result.Failure("Quantity must be greater than 0");
        }

        item.CurrentQuantity += request.QuantityAdded;
        item.LastRestockedAt = DateTime.UtcNow;

        if (request.UnitCost.HasValue)
        {
            item.UnitCost = request.UnitCost.Value;
        }

        var staffId = await _context.Staff
            .Where(s => s.UserId == _currentUserService.UserId)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        _context.InventoryTransactions.Add(new InventoryTransaction
        {
            InventoryItemId = item.Id,
            TransactionType = InventoryTransactionType.Restock,
            QuantityChange = request.QuantityAdded,
            QuantityAfter = item.CurrentQuantity,
            UnitCost = request.UnitCost ?? item.UnitCost,
            Notes = request.Notes,
            StaffId = staffId
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
