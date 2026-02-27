using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.System.Commands.ClearCache;

public record ClearCacheCommand : IRequest<Result>;
