using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.CreateCategory;

public record CreateCategoryCommand : IRequest<Result>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int DisplayOrder { get; init; } = 0;
    public bool IsActive { get; init; } = true;
}
