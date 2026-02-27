namespace RestaurantSystem.Application.Features.System.DTOs;

public class AuditLogDto
{
    public long Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public int RecordId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
