using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities.Identity;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.EmailConfirmationTokenHash)
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(u => u.EmailConfirmationTokenExpiresAt)
            .IsRequired(false);

        builder.Property(u => u.LastConfirmationSentAt)
            .IsRequired(false);

        builder.Property(u => u.PasswordResetTokenHash)
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(u => u.PasswordResetTokenExpiresAt)
            .IsRequired(false);

        builder.Property(u => u.LastPasswordResetSentAt)
            .IsRequired(false);

        builder.Property(u => u.PendingEmail)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(u => u.EmailChangeTokenHash)
            .HasMaxLength(256)
            .IsRequired(false);

        builder.Property(u => u.EmailChangeTokenExpiresAt)
            .IsRequired(false);

        builder.Property(u => u.LastEmailChangeSentAt)
            .IsRequired(false);

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
