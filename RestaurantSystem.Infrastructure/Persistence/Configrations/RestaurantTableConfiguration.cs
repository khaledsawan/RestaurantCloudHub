using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class RestaurantTableConfiguration : IEntityTypeConfiguration<RestaurantTable>
{
    public void Configure(EntityTypeBuilder<RestaurantTable> builder)
    {
        builder.ToTable("restaurant_tables");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("table_id");

        builder.Property(t => t.TableNumber)
            .HasColumnName("table_number")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(t => t.Capacity)
            .HasColumnName("capacity")
            .IsRequired();

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<TableStatus>(v, true));

        builder.Property(t => t.Location)
            .HasColumnName("location")
            .HasMaxLength(50);

        builder.Property(t => t.QrCodeUrl)
            .HasColumnName("qr_code_url")
            .HasMaxLength(500);

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at");

        builder.Ignore(t => t.UpdatedAt);

        builder.HasIndex(t => t.TableNumber)
            .IsUnique();
    }
}
