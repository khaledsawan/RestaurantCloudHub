using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("inventory_item_id");

        builder.Property(i => i.InventoryCategoryId)
            .HasColumnName("inventory_category_id");

        builder.Property(i => i.Sku)
            .HasColumnName("sku")
            .HasMaxLength(50);

        builder.Property(i => i.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.UnitOfMeasure)
            .HasColumnName("unit_of_measure")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(i => i.CurrentQuantity)
            .HasColumnName("current_quantity")
            .HasPrecision(10, 2);

        builder.Property(i => i.MinimumQuantity)
            .HasColumnName("minimum_quantity")
            .HasPrecision(10, 2);

        builder.Property(i => i.ReorderQuantity)
            .HasColumnName("reorder_quantity")
            .HasPrecision(10, 2);

        builder.Property(i => i.UnitCost)
            .HasColumnName("unit_cost")
            .HasPrecision(10, 2);

        builder.Property(i => i.SupplierName)
            .HasColumnName("supplier_name")
            .HasMaxLength(200);

        builder.Property(i => i.SupplierContact)
            .HasColumnName("supplier_contact")
            .HasMaxLength(200);

        builder.Property(i => i.LastRestockedAt)
            .HasColumnName("last_restocked_at");

        builder.Property(i => i.NextRestockDate)
            .HasColumnName("next_restock_date")
            .HasColumnType("date");

        builder.Property(i => i.IsActive)
            .HasColumnName("is_active");

        builder.Property(i => i.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(i => i.InventoryCategoryId)
            .HasDatabaseName("idx_inventory_items_category")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(i => i.CurrentQuantity)
            .HasDatabaseName("idx_inventory_items_low_stock")
            .HasFilter("\"current_quantity\" <= \"minimum_quantity\" AND \"deleted_at\" IS NULL");

        builder.HasQueryFilter(i => i.DeletedAt == null);

        builder.HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.InventoryCategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
