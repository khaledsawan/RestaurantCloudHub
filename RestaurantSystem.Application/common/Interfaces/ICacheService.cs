namespace RestaurantSystem.Application.Common.Interfaces;

public interface ICacheService
{
    Task ClearAsync(CancellationToken cancellationToken = default);
}
