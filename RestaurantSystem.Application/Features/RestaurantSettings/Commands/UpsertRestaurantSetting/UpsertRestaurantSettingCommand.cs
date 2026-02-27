using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.RestaurantSettings.Commands.UpsertRestaurantSetting;

public class UpsertRestaurantSettingCommand : IRequest<Result>
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = "{}";
}
