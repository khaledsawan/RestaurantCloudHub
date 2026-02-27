using MediatR;

namespace RestaurantSystem.Application.Features.RestaurantSettings.Queries.GetRestaurantSetting;

public record GetRestaurantSettingQuery(string Key) : IRequest<string?>;
