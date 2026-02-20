using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(c => c.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.DateOfBirth)
            .HasColumnName("date_of_birth");

        builder.Property(c => c.ProfileImageUrl)
            .HasColumnName("profile_image_url")
            .HasMaxLength(500);

        builder.Property(c => c.LoyaltyPoints)
            .HasColumnName("loyalty_points");

        builder.Property(c => c.TotalOrders)
            .HasColumnName("total_orders");

        builder.Property(c => c.TotalSpent)
            .HasColumnName("total_spent")
            .HasPrecision(10, 2);

        builder.Property(c => c.AverageRating)
            .HasColumnName("average_rating")
            .HasPrecision(3, 2);

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active");

        builder.Property(c => c.IsVerified)
            .HasColumnName("is_verified");

        builder.Property(c => c.LastLoginAt)
            .HasColumnName("last_login_at");

        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(c => c.CreatedById)
            .HasColumnName("created_by_id");

        builder.Property(c => c.UpdatedById)
            .HasColumnName("updated_by_id");

        builder.HasIndex(c => c.Email)
            .IsUnique();

        builder.HasIndex(c => c.Email)
            .HasDatabaseName("idx_customers_email")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasIndex(c => c.Phone)
            .HasDatabaseName("idx_customers_phone")
            .HasFilter("\"deleted_at\" IS NULL");

        builder.HasQueryFilter(c => c.DeletedAt == null);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Addresses)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
