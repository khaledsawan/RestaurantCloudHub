using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class MenuItemOptionConfiguration : IEntityTypeConfiguration<MenuItemOption>
{
    public void Configure(EntityTypeBuilder<MenuItemOption> builder)
    {
        builder.ToTable("menu_item_options");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("option_id");

        builder.Property(o => o.OptionGroupId)
            .HasColumnName("option_group_id")
            .IsRequired();

        builder.Property(o => o.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(o => o.PriceAdjustment)
            .HasColumnName("price_adjustment")
            .HasPrecision(10, 2);

        builder.Property(o => o.CaloriesAdjustment)
            .HasColumnName("calories_adjustment");

        builder.Property(o => o.IsAvailable)
            .HasColumnName("is_available");

        builder.Property(o => o.IsDefault)
            .HasColumnName("is_default");

        builder.Property(o => o.DisplayOrder)
            .HasColumnName("display_order");

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at");

        builder.Ignore(o => o.UpdatedAt);

        builder.HasOne(o => o.OptionGroup)
            .WithMany(g => g.Options)
            .HasForeignKey(o => o.OptionGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(o => o.OptionGroup.MenuItem.DeletedAt == null);
    }
}
