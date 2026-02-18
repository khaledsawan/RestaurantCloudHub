using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantSystem.Domain.Entities;

/// <summary>
/// Audit log entry for tracking changes
/// </summary>
[Table("audit_logs")]
public class AuditLog
{
    [Key]
    [Column("audit_id")]
    public long AuditId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("table_name")]
    public string TableName { get; set; } = string.Empty;

    [Required]
    [Column("record_id")]
    public int RecordId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("action")]
    public string Action { get; set; } = string.Empty; // INSERT, UPDATE, DELETE

    [Column("old_values", TypeName = "jsonb")]
    public string? OldValues { get; set; }

    [Column("new_values", TypeName = "jsonb")]
    public string? NewValues { get; set; }

    [Column("changed_by")]
    public int? ChangedById { get; set; }

    [MaxLength(20)]
    [Column("changed_by_type")]
    public string? ChangedByType { get; set; }

    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
