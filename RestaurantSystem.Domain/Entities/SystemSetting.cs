using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RestaurantSystem.Domain.Common;

namespace RestaurantSystem.Domain.Entities;

[Table("system_settings")]
public class SystemSetting : BaseEntity
{
    [Required]
    [MaxLength(100)]
    [Column("key")]
    public string Key { get; set; } = string.Empty;

    [Required]
    [Column("value", TypeName = "jsonb")]
    public string Value { get; set; } = "{}";

    [MaxLength(500)]
    [Column("description")]
    public string? Description { get; set; }

    [Column("updated_by_id")]
    public int? UpdatedById { get; set; }
}
