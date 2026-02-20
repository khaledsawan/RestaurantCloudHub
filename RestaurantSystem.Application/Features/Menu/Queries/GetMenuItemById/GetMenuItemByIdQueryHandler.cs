using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Menu.DTOs;

namespace RestaurantSystem.Application.Features.Menu.Queries.GetMenuItemById;

public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, MenuItemDetailDto?>
{
    private readonly IApplicationDbContext _context;

    public GetMenuItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuItemDetailDto?> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.MenuItems
            .AsNoTracking()
            .Include(m => m.Category)
            .Include(m => m.OptionGroups)
                .ThenInclude(g => g.Options)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (item == null)
        {
            return null;
        }

        return new MenuItemDetailDto
        {
            Id = item.Id,
            CategoryId = item.CategoryId,
            CategoryName = item.Category.Name,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            ImageUrl = item.ImageUrl,
            ThumbnailUrl = item.ThumbnailUrl,
            IsAvailable = item.IsAvailable,
            IsFeatured = item.IsFeatured,
            PreparationTimeMinutes = item.PreparationTimeMinutes,
            Calories = item.Calories,
            SpiceLevel = item.SpiceLevel,
            IsVegetarian = item.IsVegetarian,
            IsVegan = item.IsVegan,
            IsGlutenFree = item.IsGlutenFree,
            IsDairyFree = item.IsDairyFree,
            IsNutFree = item.IsNutFree,
            AllergenInfo = item.AllergenInfo,
            MaxQuantityPerOrder = item.MaxQuantityPerOrder,
            OptionGroups = item.OptionGroups
                .OrderBy(g => g.DisplayOrder)
                .Select(g => new OptionGroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    IsRequired = g.IsRequired,
                    SelectionType = g.SelectionType,
                    MinSelections = g.MinSelections,
                    MaxSelections = g.MaxSelections,
                    DisplayOrder = g.DisplayOrder,
                    Options = g.Options
                        .OrderBy(o => o.DisplayOrder)
                        .Select(o => new OptionDto
                        {
                            Id = o.Id,
                            Name = o.Name,
                            PriceAdjustment = o.PriceAdjustment,
                            CaloriesAdjustment = o.CaloriesAdjustment,
                            IsAvailable = o.IsAvailable,
                            IsDefault = o.IsDefault,
                            DisplayOrder = o.DisplayOrder
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}
