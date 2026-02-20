using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Features.Menu.Commands.CreateOptionGroup;

public class CreateOptionGroupCommandHandler : IRequestHandler<CreateOptionGroupCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public CreateOptionGroupCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(CreateOptionGroupCommand request, CancellationToken cancellationToken)
    {
        var itemExists = await _context.MenuItems
            .AnyAsync(m => m.Id == request.ItemId, cancellationToken);

        if (!itemExists)
        {
            return Result.Failure("Menu item not found");
        }

        var group = new MenuItemOptionGroup
        {
            ItemId = request.ItemId,
            Name = request.Name,
            Description = request.Description,
            IsRequired = request.IsRequired,
            SelectionType = request.SelectionType,
            MinSelections = request.MinSelections,
            MaxSelections = request.MaxSelections,
            DisplayOrder = request.DisplayOrder
        };

        _context.MenuItemOptionGroups.Add(group);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
