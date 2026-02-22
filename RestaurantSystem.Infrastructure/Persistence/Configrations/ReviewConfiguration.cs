using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("review_id");

        builder.Property(r => r.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(r => r.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(r => r.Rating)
            .HasColumnName("rating")
            .IsRequired();

        builder.Property(r => r.FoodRating)
            .HasColumnName("food_rating");

        builder.Property(r => r.ServiceRating)
            .HasColumnName("service_rating");

        builder.Property(r => r.DeliveryRating)
            .HasColumnName("delivery_rating");

        builder.Property(r => r.ReviewText)
            .HasColumnName("review_text");

        builder.Property(r => r.ResponseText)
            .HasColumnName("response_text");

        builder.Property(r => r.RespondedById)
            .HasColumnName("responded_by_id");

        builder.Property(r => r.RespondedAt)
            .HasColumnName("responded_at");

        builder.Property(r => r.IsPublished)
            .HasColumnName("is_published");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(r => r.CustomerId)
            .HasDatabaseName("idx_reviews_customer");

        builder.HasIndex(r => r.Rating)
            .HasDatabaseName("idx_reviews_rating");

        builder.HasOne(r => r.Order)
            .WithMany()
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(r => r.Customer.DeletedAt == null);

        builder.HasOne(r => r.RespondedBy)
            .WithMany()
            .HasForeignKey(r => r.RespondedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
