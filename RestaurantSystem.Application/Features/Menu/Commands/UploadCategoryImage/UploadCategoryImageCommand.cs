using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.UploadCategoryImage;

public record UploadCategoryImageCommand : IRequest<Result<string>>
{
    public int CategoryId { get; init; }
    public byte[] Content { get; init; } = Array.Empty<byte>();
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
