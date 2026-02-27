using MediatR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.System.Commands.ClearCache;

public class ClearCacheCommandHandler : IRequestHandler<ClearCacheCommand, Result>
{
    private readonly ICacheService _cacheService;

    public ClearCacheCommandHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(ClearCacheCommand request, CancellationToken cancellationToken)
    {
        await _cacheService.ClearAsync(cancellationToken);
        return Result.Success();
    }
}
