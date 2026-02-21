using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class OrderItemOptionConfiguration : IEntityTypeConfiguration<OrderItemOption>
{
    public void Configure(EntityTypeBuilder<OrderItemOption> builder)
    {
        builder.ToTable("order_item_options");
        builder.HasKey(o => o.OrderItemOptionId);

        builder.Property(o => o.OrderItemOptionId)
            .HasColumnName("order_item_option_id");

        builder.Property(o => o.OrderItemId)
            .HasColumnName("order_item_id")
            .IsRequired();

        builder.Property(o => o.OptionId)
            .HasColumnName("option_id")
            .IsRequired();

        builder.Property(o => o.OptionGroupName)
            .HasColumnName("option_group_name")
            .HasMaxLength(100);

        builder.Property(o => o.OptionName)
            .HasColumnName("option_name")
            .HasMaxLength(100);

        builder.Property(o => o.PriceAdjustment)
            .HasColumnName("price_adjustment")
            .HasPrecision(10, 2);

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at");

        builder.HasIndex(o => o.OrderItemId)
            .HasDatabaseName("idx_order_item_options_order_item");

        builder.HasOne(o => o.OrderItem)
            .WithMany(i => i.SelectedOptions)
            .HasForeignKey(o => o.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Option)
            .WithMany()
            .HasForeignKey(o => o.OptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(o => o.Option.OptionGroup.MenuItem.DeletedAt == null);
    }
}
