using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Features.Menu.Commands.CreateMenuItem;

public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public CreateMenuItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            return Result.Failure("Category not found");
        }

        var item = new MenuItem
        {
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            ThumbnailUrl = request.ThumbnailUrl,
            IsAvailable = request.IsAvailable,
            IsFeatured = request.IsFeatured,
            PreparationTimeMinutes = request.PreparationTimeMinutes,
            Calories = request.Calories,
            SpiceLevel = request.SpiceLevel,
            IsVegetarian = request.IsVegetarian,
            IsVegan = request.IsVegan,
            IsGlutenFree = request.IsGlutenFree,
            IsDairyFree = request.IsDairyFree,
            IsNutFree = request.IsNutFree,
            AllergenInfo = request.AllergenInfo,
            MaxQuantityPerOrder = request.MaxQuantityPerOrder
        };

        _context.MenuItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
