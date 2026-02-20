using MediatR;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Menu.Commands.CreateOptionGroup;

public record CreateOptionGroupCommand : IRequest<Result>
{
    public int ItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsRequired { get; init; } = false;
    public OptionSelectionType SelectionType { get; init; } = OptionSelectionType.Single;
    public int MinSelections { get; init; } = 0;
    public int MaxSelections { get; init; } = 1;
    public int DisplayOrder { get; init; } = 0;
}
