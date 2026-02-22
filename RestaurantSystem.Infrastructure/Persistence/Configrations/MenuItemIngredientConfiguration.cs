using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class MenuItemIngredientConfiguration : IEntityTypeConfiguration<MenuItemIngredient>
{
    public void Configure(EntityTypeBuilder<MenuItemIngredient> builder)
    {
        builder.ToTable("menu_item_ingredients");
        builder.HasKey(m => m.MenuItemIngredientId);

        builder.Property(m => m.MenuItemIngredientId)
            .HasColumnName("menu_item_ingredient_id");

        builder.Property(m => m.ItemId)
            .HasColumnName("item_id")
            .IsRequired();

        builder.Property(m => m.InventoryItemId)
            .HasColumnName("inventory_item_id")
            .IsRequired();

        builder.Property(m => m.QuantityRequired)
            .HasColumnName("quantity_required")
            .HasPrecision(10, 3);

        builder.HasIndex(m => new { m.ItemId, m.InventoryItemId })
            .IsUnique();

        builder.HasOne(m => m.MenuItem)
            .WithMany()
            .HasForeignKey(m => m.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.InventoryItem)
            .WithMany()
            .HasForeignKey(m => m.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(m => m.InventoryItem.DeletedAt == null);
    }
}
