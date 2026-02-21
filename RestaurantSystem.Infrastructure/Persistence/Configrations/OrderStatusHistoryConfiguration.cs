using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("order_status_history");
        builder.HasKey(h => h.HistoryId);

        builder.Property(h => h.HistoryId)
            .HasColumnName("history_id");

        builder.Property(h => h.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(h => h.FromStatus)
            .HasColumnName("from_status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.HasValue ? v.Value.ToString().ToLowerInvariant() : null,
                v => string.IsNullOrWhiteSpace(v) ? null : Enum.Parse<OrderStatus>(v, true));

        builder.Property(h => h.ToStatus)
            .HasColumnName("to_status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<OrderStatus>(v, true));

        builder.Property(h => h.ChangedById)
            .HasColumnName("changed_by_id");

        builder.Property(h => h.ChangedByType)
            .HasColumnName("changed_by_type")
            .HasMaxLength(20);

        builder.Property(h => h.Notes)
            .HasColumnName("notes");

        builder.Property(h => h.CreatedAt)
            .HasColumnName("created_at");

        builder.HasIndex(h => h.OrderId)
            .HasDatabaseName("idx_order_status_history_order");

        builder.HasOne(h => h.Order)
            .WithMany(o => o.StatusHistory)
            .HasForeignKey(h => h.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
