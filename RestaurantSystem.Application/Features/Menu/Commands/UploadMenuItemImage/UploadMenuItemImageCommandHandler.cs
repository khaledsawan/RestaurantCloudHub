using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.UploadMenuItemImage;

public class UploadMenuItemImageCommandHandler : IRequestHandler<UploadMenuItemImageCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;

    public UploadMenuItemImageCommandHandler(IApplicationDbContext context, IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Result<string>> Handle(UploadMenuItemImageCommand request, CancellationToken cancellationToken)
    {
        if (request.ItemId <= 0)
        {
            return Result<string>.Failure("ItemId is required");
        }

        if (request.Content.Length == 0)
        {
            return Result<string>.Failure("Empty file");
        }

        if (!request.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return Result<string>.Failure("Only image uploads are allowed");
        }

        var item = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.Id == request.ItemId, cancellationToken);

        if (item == null)
        {
            return Result<string>.Failure("Menu item not found");
        }

        var extension = Path.GetExtension(request.FileName);
        var safeName = $"{Guid.NewGuid():N}{extension}";
        var url = await _fileStorage.SaveAsync(request.Content, safeName, request.ContentType, "uploads/menu", cancellationToken);

        item.ImageUrl = url;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(url);
    }
}
