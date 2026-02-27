using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.System.Commands.UpsertSystemSetting;

public class UpsertSystemSettingCommand : IRequest<Result>
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = "{}";
    public string? Description { get; set; }
}
