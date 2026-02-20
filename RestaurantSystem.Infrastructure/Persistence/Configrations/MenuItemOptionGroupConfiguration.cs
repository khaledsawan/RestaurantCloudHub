using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class MenuItemOptionGroupConfiguration : IEntityTypeConfiguration<MenuItemOptionGroup>
{
    public void Configure(EntityTypeBuilder<MenuItemOptionGroup> builder)
    {
        builder.ToTable("menu_item_option_groups");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("option_group_id");

        builder.Property(g => g.ItemId)
            .HasColumnName("item_id")
            .IsRequired();

        builder.Property(g => g.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(g => g.Description)
            .HasColumnName("description");

        builder.Property(g => g.IsRequired)
            .HasColumnName("is_required");

        builder.Property(g => g.SelectionType)
            .HasColumnName("selection_type")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<RestaurantSystem.Domain.Enums.OptionSelectionType>(v, true));

        builder.Property(g => g.MinSelections)
            .HasColumnName("min_selections");

        builder.Property(g => g.MaxSelections)
            .HasColumnName("max_selections");

        builder.Property(g => g.DisplayOrder)
            .HasColumnName("display_order");

        builder.Property(g => g.CreatedAt)
            .HasColumnName("created_at");

        builder.Ignore(g => g.UpdatedAt);

        builder.HasIndex(g => g.ItemId)
            .HasDatabaseName("idx_option_groups_item");

        builder.HasOne(g => g.MenuItem)
            .WithMany(m => m.OptionGroups)
            .HasForeignKey(g => g.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(g => g.MenuItem.DeletedAt == null);
    }
}
