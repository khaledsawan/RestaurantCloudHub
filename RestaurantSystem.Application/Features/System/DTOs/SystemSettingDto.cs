using System.Text.Json;

namespace RestaurantSystem.Application.Features.System.DTOs;

public class SystemSettingDto
{
    public string Key { get; set; } = string.Empty;
    public JsonElement Value { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
}
