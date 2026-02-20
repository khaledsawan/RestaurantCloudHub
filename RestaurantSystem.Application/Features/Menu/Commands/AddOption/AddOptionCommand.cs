using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.AddOption;

public record AddOptionCommand : IRequest<Result>
{
    public int OptionGroupId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal PriceAdjustment { get; init; } = 0;
    public int CaloriesAdjustment { get; init; } = 0;
    public bool IsAvailable { get; init; } = true;
    public bool IsDefault { get; init; } = false;
    public int DisplayOrder { get; init; } = 0;
}
