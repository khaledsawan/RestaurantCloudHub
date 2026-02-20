namespace RestaurantSystem.Application.Common.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(byte[] content, string fileName, string contentType, string relativeFolder, CancellationToken cancellationToken);
}
