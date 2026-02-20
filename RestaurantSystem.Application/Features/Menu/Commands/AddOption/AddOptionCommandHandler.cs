using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Features.Menu.Commands.AddOption;

public class AddOptionCommandHandler : IRequestHandler<AddOptionCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AddOptionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(AddOptionCommand request, CancellationToken cancellationToken)
    {
        var groupExists = await _context.MenuItemOptionGroups
            .AnyAsync(g => g.Id == request.OptionGroupId, cancellationToken);

        if (!groupExists)
        {
            return Result.Failure("Option group not found");
        }

        var option = new MenuItemOption
        {
            OptionGroupId = request.OptionGroupId,
            Name = request.Name,
            PriceAdjustment = request.PriceAdjustment,
            CaloriesAdjustment = request.CaloriesAdjustment,
            IsAvailable = request.IsAvailable,
            IsDefault = request.IsDefault,
            DisplayOrder = request.DisplayOrder
        };

        _context.MenuItemOptions.Add(option);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
