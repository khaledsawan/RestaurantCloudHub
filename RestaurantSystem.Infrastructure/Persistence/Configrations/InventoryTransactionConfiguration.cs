using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("inventory_transactions");
        builder.HasKey(t => t.TransactionId);

        builder.Property(t => t.TransactionId)
            .HasColumnName("transaction_id");

        builder.Property(t => t.InventoryItemId)
            .HasColumnName("inventory_item_id")
            .IsRequired();

        builder.Property(t => t.TransactionType)
            .HasColumnName("transaction_type")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<InventoryTransactionType>(v, true));

        builder.Property(t => t.QuantityChange)
            .HasColumnName("quantity_change")
            .HasPrecision(10, 2);

        builder.Property(t => t.QuantityAfter)
            .HasColumnName("quantity_after")
            .HasPrecision(10, 2);

        builder.Property(t => t.UnitCost)
            .HasColumnName("unit_cost")
            .HasPrecision(10, 2);

        builder.Property(t => t.ReferenceId)
            .HasColumnName("reference_id");

        builder.Property(t => t.ReferenceType)
            .HasColumnName("reference_type")
            .HasMaxLength(50);

        builder.Property(t => t.Notes)
            .HasColumnName("notes");

        builder.Property(t => t.StaffId)
            .HasColumnName("staff_id");

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at");

        builder.HasIndex(t => t.InventoryItemId)
            .HasDatabaseName("idx_inventory_transactions_item");

        builder.HasOne(t => t.InventoryItem)
            .WithMany(i => i.Transactions)
            .HasForeignKey(t => t.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(t => t.InventoryItem.DeletedAt == null);

        builder.HasOne(t => t.Staff)
            .WithMany()
            .HasForeignKey(t => t.StaffId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
