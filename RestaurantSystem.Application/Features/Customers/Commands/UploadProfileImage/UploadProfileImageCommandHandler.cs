using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Customers.Commands.UploadProfileImage;

public class UploadProfileImageCommandHandler : IRequestHandler<UploadProfileImageCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorage _fileStorage;

    public UploadProfileImageCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorage fileStorage)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorage = fileStorage;
    }

    public async Task<Result<string>> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Result<string>.Failure("User not authenticated");
        }

        if (request.Content.Length == 0)
        {
            return Result<string>.Failure("Empty file");
        }

        if (!request.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return Result<string>.Failure("Only image uploads are allowed");
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId.Value, cancellationToken);

        if (customer == null)
        {
            return Result<string>.Failure("Customer profile not found");
        }

        var extension = Path.GetExtension(request.FileName);
        var safeName = $"{Guid.NewGuid():N}{extension}";
        var url = await _fileStorage.SaveAsync(request.Content, safeName, request.ContentType, "uploads/avatars", cancellationToken);

        customer.ProfileImageUrl = url;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(url);
    }
}
