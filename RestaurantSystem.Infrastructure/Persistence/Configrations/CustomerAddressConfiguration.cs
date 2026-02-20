using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("customer_addresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(a => a.Label)
            .HasColumnName("label")
            .HasMaxLength(50);

        builder.Property(a => a.AddressLine1)
            .HasColumnName("address_line1")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.AddressLine2)
            .HasColumnName("address_line2")
            .HasMaxLength(255);

        builder.Property(a => a.City)
            .HasColumnName("city")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.State)
            .HasColumnName("state")
            .HasMaxLength(50);

        builder.Property(a => a.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.Country)
            .HasColumnName("country")
            .HasMaxLength(100)
            .HasDefaultValue("USA");

        builder.Property(a => a.Latitude)
            .HasColumnName("latitude")
            .HasPrecision(10, 8);

        builder.Property(a => a.Longitude)
            .HasColumnName("longitude")
            .HasPrecision(11, 8);

        builder.Property(a => a.DeliveryInstructions)
            .HasColumnName("delivery_instructions");

        builder.Property(a => a.IsDefault)
            .HasColumnName("is_default");

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(a => a.CustomerId)
            .HasDatabaseName("idx_customer_addresses_customer");

        // Ensure addresses for soft-deleted customers are filtered out too
        builder.HasQueryFilter(a => a.Customer.DeletedAt == null);
    }
}
