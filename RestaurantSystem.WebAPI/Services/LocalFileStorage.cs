using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.WebAPI.Services;

public class LocalFileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorage(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(byte[] content, string fileName, string contentType, string relativeFolder, CancellationToken cancellationToken)
    {
        var webRoot = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        Directory.CreateDirectory(webRoot);
        var folderPath = Path.Combine(webRoot, relativeFolder);
        Directory.CreateDirectory(folderPath);

        var safeName = Path.GetFileName(fileName);
        var targetPath = Path.Combine(folderPath, safeName);

        await File.WriteAllBytesAsync(targetPath, content, cancellationToken);

        return $"/{relativeFolder.Replace("\\", "/")}/{safeName}";
    }
}
