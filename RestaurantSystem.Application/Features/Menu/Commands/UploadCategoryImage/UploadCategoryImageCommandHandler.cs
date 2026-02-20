using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.UploadCategoryImage;

public class UploadCategoryImageCommandHandler : IRequestHandler<UploadCategoryImageCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;

    public UploadCategoryImageCommandHandler(IApplicationDbContext context, IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Result<string>> Handle(UploadCategoryImageCommand request, CancellationToken cancellationToken)
    {
        if (request.CategoryId <= 0)
        {
            return Result<string>.Failure("CategoryId is required");
        }

        if (request.Content.Length == 0)
        {
            return Result<string>.Failure("Empty file");
        }

        if (!request.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return Result<string>.Failure("Only image uploads are allowed");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category == null)
        {
            return Result<string>.Failure("Category not found");
        }

        var extension = Path.GetExtension(request.FileName);
        var safeName = $"{Guid.NewGuid():N}{extension}";
        var url = await _fileStorage.SaveAsync(request.Content, safeName, request.ContentType, "uploads/menu/categories", cancellationToken);

        category.ImageUrl = url;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(url);
    }
}
