using Microsoft.AspNetCore.Http;

namespace RestaurantSystem.WebAPI.Models;

public class UploadProfileImageRequest
{
    public IFormFile File { get; set; } = null!;
}
