using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.UpdateMenuItem;

public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateMenuItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (item == null)
        {
            return Result.Failure("Menu item not found");
        }

        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId.Value, cancellationToken);

            if (!categoryExists)
            {
                return Result.Failure("Category not found");
            }

            item.CategoryId = request.CategoryId.Value;
        }

        if (request.Name != null)
        {
            item.Name = request.Name;
        }

        if (request.Description != null)
        {
            item.Description = request.Description;
        }

        if (request.Price.HasValue)
        {
            item.Price = request.Price.Value;
        }

        if (request.ImageUrl != null)
        {
            item.ImageUrl = request.ImageUrl;
        }

        if (request.ThumbnailUrl != null)
        {
            item.ThumbnailUrl = request.ThumbnailUrl;
        }

        if (request.IsAvailable.HasValue)
        {
            item.IsAvailable = request.IsAvailable.Value;
        }

        if (request.IsFeatured.HasValue)
        {
            item.IsFeatured = request.IsFeatured.Value;
        }

        if (request.PreparationTimeMinutes.HasValue)
        {
            item.PreparationTimeMinutes = request.PreparationTimeMinutes.Value;
        }

        if (request.Calories.HasValue)
        {
            item.Calories = request.Calories.Value;
        }

        if (request.SpiceLevel.HasValue)
        {
            item.SpiceLevel = request.SpiceLevel.Value;
        }

        if (request.IsVegetarian.HasValue)
        {
            item.IsVegetarian = request.IsVegetarian.Value;
        }

        if (request.IsVegan.HasValue)
        {
            item.IsVegan = request.IsVegan.Value;
        }

        if (request.IsGlutenFree.HasValue)
        {
            item.IsGlutenFree = request.IsGlutenFree.Value;
        }

        if (request.IsDairyFree.HasValue)
        {
            item.IsDairyFree = request.IsDairyFree.Value;
        }

        if (request.IsNutFree.HasValue)
        {
            item.IsNutFree = request.IsNutFree.Value;
        }

        if (request.AllergenInfo != null)
        {
            item.AllergenInfo = request.AllergenInfo;
        }

        if (request.MaxQuantityPerOrder.HasValue)
        {
            item.MaxQuantityPerOrder = request.MaxQuantityPerOrder.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
