using System.Text.Json;

namespace RestaurantSystem.WebAPI.Models;

public class UpsertSystemSettingRequest
{
    public JsonElement Value { get; set; }
    public string? Description { get; set; }
}
