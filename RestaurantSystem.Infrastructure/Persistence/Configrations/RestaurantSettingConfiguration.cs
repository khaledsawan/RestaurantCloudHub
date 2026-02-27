using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class RestaurantSettingConfiguration : IEntityTypeConfiguration<RestaurantSetting>
{
    public void Configure(EntityTypeBuilder<RestaurantSetting> builder)
    {
        builder.ToTable("restaurant_settings");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Key)
            .HasColumnName("key")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasColumnName("value")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.UpdatedById)
            .HasColumnName("updated_by_id");

        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(x => x.Key)
            .IsUnique()
            .HasDatabaseName("ix_restaurant_settings_key");
    }
}
