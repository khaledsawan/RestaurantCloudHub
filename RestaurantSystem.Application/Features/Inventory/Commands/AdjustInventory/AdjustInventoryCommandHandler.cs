using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Inventory.Commands.AdjustInventory;

public class AdjustInventoryCommandHandler : IRequestHandler<AdjustInventoryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AdjustInventoryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(AdjustInventoryCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == request.InventoryItemId, cancellationToken);

        if (item == null)
        {
            return Result.Failure("Inventory item not found");
        }

        var newQuantity = item.CurrentQuantity + request.QuantityChange;
        if (newQuantity < 0)
        {
            return Result.Failure("Inventory quantity cannot be negative");
        }

        item.CurrentQuantity = newQuantity;

        var staffId = await _context.Staff
            .Where(s => s.UserId == _currentUserService.UserId)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        _context.InventoryTransactions.Add(new InventoryTransaction
        {
            InventoryItemId = item.Id,
            TransactionType = InventoryTransactionType.Adjustment,
            QuantityChange = request.QuantityChange,
            QuantityAfter = item.CurrentQuantity,
            Notes = request.Notes,
            StaffId = staffId
        });

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
