using MediatR;
using RestaurantSystem.Application.Features.System.DTOs;

namespace RestaurantSystem.Application.Features.System.Queries.GetSystemSettingByKey;

public record GetSystemSettingByKeyQuery(string Key) : IRequest<SystemSettingDto?>;
