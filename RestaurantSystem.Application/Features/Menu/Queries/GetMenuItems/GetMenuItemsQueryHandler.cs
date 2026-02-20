using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Menu.DTOs;

namespace RestaurantSystem.Application.Features.Menu.Queries.GetMenuItems;

public class GetMenuItemsQueryHandler : IRequestHandler<GetMenuItemsQuery, List<MenuItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMenuItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuItemDto>> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.MenuItems
            .AsNoTracking()
            .Include(m => m.Category)
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(m => m.CategoryId == request.CategoryId.Value);
        }

        if (request.AvailableOnly == true)
        {
            query = query.Where(m => m.IsAvailable);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(m => m.Name.Contains(term) || (m.Description != null && m.Description.Contains(term)));
        }

        return await query
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
