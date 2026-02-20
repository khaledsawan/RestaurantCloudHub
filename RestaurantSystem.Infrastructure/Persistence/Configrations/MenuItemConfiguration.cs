using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("item_id");

        builder.Property(m => m.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(m => m.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasColumnName("description");

        builder.Property(m => m.Price)
            .HasColumnName("price")
            .HasPrecision(10, 2);

        builder.Property(m => m.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(500);

        builder.Property(m => m.ThumbnailUrl)
            .HasColumnName("thumbnail_url")
            .HasMaxLength(500);

        builder.Property(m => m.IsAvailable)
            .HasColumnName("is_available");

        builder.Property(m => m.IsFeatured)
            .HasColumnName("is_featured");

        builder.Property(m => m.PreparationTimeMinutes)
            .HasColumnName("preparation_time_minutes");

        builder.Property(m => m.Calories)
            .HasColumnName("calories");

        builder.Property(m => m.SpiceLevel)
            .HasColumnName("spice_level");

        builder.Property(m => m.IsVegetarian)
            .HasColumnName("is_vegetarian");

        builder.Property(m => m.IsVegan)
            .HasColumnName("is_vegan");

        builder.Property(m => m.IsGlutenFree)
            .HasColumnName("is_gluten_free");

        builder.Property(m => m.IsDairyFree)
            .HasColumnName("is_dairy_free");

        builder.Property(m => m.IsNutFree)
            .HasColumnName("is_nut_free");

        builder.Property(m => m.AllergenInfo)
            .HasColumnName("allergen_info");

        builder.Property(m => m.MaxQuantityPerOrder)
            .HasColumnName("max_quantity_per_order");

        builder.Property(m => m.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(m => m.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(m => m.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(m => m.CreatedById)
            .HasColumnName("created_by_id");

        builder.Property(m => m.UpdatedById)
            .HasColumnName("updated_by_id");

        builder.HasIndex(m => m.CategoryId)
            .HasDatabaseName("idx_menu_items_category")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(m => m.IsAvailable)
            .HasDatabaseName("idx_menu_items_available")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(m => m.IsFeatured)
            .HasDatabaseName("idx_menu_items_featured")
            .HasFilter("\"is_featured\" = TRUE AND \"deleted_at\" IS NULL");

        builder.HasQueryFilter(m => m.DeletedAt == null);

        builder.HasOne(m => m.Category)
            .WithMany(c => c.MenuItems)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
