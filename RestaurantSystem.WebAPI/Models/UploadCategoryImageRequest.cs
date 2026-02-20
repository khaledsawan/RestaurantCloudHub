using Microsoft.AspNetCore.Http;

namespace RestaurantSystem.WebAPI.Models;

public class UploadCategoryImageRequest
{
    public IFormFile? File { get; set; }
}
