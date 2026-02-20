using Microsoft.AspNetCore.Http;

namespace RestaurantSystem.WebAPI.Models;

public class UploadMenuItemImageRequest
{
    public IFormFile? File { get; set; }
}
