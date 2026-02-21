using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.ToTable("staff");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("staff_id");

        builder.Property(s => s.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(s => s.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(s => s.ProfileImageUrl)
            .HasColumnName("profile_image_url")
            .HasMaxLength(500);

        builder.Property(s => s.IsActive)
            .HasColumnName("is_active");

        builder.Property(s => s.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(s => s.CreatedById)
            .HasColumnName("created_by_id");

        builder.Property(s => s.UpdatedById)
            .HasColumnName("updated_by_id");

        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasDatabaseName("idx_staff_user");

        builder.HasQueryFilter(s => s.DeletedAt == null);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
