using Microsoft.Extensions.Caching.Memory;
using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.Infrastructure.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        if (_cache is MemoryCache memoryCache)
        {
            memoryCache.Compact(1.0);
        }

        return Task.CompletedTask;
    }
}
