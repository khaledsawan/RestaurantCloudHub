using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities.Identity;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(ur => ur.Id);

        builder.Property(ur => ur.RoleName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(ur => new { ur.UserId, ur.RoleName })
            .IsUnique();
    }
}