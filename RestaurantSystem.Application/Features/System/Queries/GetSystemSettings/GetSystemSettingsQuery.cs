using MediatR;
using RestaurantSystem.Application.Features.System.DTOs;

namespace RestaurantSystem.Application.Features.System.Queries.GetSystemSettings;

public record GetSystemSettingsQuery : IRequest<List<SystemSettingDto>>;
