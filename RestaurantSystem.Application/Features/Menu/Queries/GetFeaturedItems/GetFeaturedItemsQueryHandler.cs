using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Menu.DTOs;

namespace RestaurantSystem.Application.Features.Menu.Queries.GetFeaturedItems;

public class GetFeaturedItemsQueryHandler : IRequestHandler<GetFeaturedItemsQuery, List<MenuItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetFeaturedItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuItemDto>> Handle(GetFeaturedItemsQuery request, CancellationToken cancellationToken)
    {
        return await _context.MenuItems
            .AsNoTracking()
            .Include(m => m.Category)
            .Where(m => m.IsFeatured)
            .OrderBy(m => m.Name)
            .Select(m => new MenuItemDto
            {
                Id = m.Id,
                CategoryId = m.CategoryId,
                CategoryName = m.Category.Name,
                Name = m.Name,
                Description = m.Description,
                Price = m.Price,
                ImageUrl = m.ImageUrl,
                ThumbnailUrl = m.ThumbnailUrl,
                IsAvailable = m.IsAvailable,
                IsFeatured = m.IsFeatured,
                PreparationTimeMinutes = m.PreparationTimeMinutes,
                Calories = m.Calories,
                SpiceLevel = m.SpiceLevel,
                IsVegetarian = m.IsVegetarian,
                IsVegan = m.IsVegan,
                IsGlutenFree = m.IsGlutenFree,
                IsDairyFree = m.IsDairyFree,
                IsNutFree = m.IsNutFree,
                AllergenInfo = m.AllergenInfo,
                MaxQuantityPerOrder = m.MaxQuantityPerOrder
            })
            .ToListAsync(cancellationToken);
    }
}
