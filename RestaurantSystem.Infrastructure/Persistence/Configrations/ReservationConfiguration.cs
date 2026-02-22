using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("reservation_id");

        builder.Property(r => r.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(r => r.TableId)
            .HasColumnName("table_id");

        builder.Property(r => r.ReservationDate)
            .HasColumnName("reservation_date")
            .HasColumnType("date");

        builder.Property(r => r.ReservationTime)
            .HasColumnName("reservation_time")
            .HasColumnType("time");

        builder.Property(r => r.PartySize)
            .HasColumnName("party_size")
            .IsRequired();

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<ReservationStatus>(v, true));

        builder.Property(r => r.SpecialRequests)
            .HasColumnName("special_requests");

        builder.Property(r => r.CustomerNotes)
            .HasColumnName("customer_notes");

        builder.Property(r => r.StaffNotes)
            .HasColumnName("staff_notes");

        builder.Property(r => r.ConfirmationCode)
            .HasColumnName("confirmation_code")
            .HasMaxLength(20);

        builder.Property(r => r.RemindedAt)
            .HasColumnName("reminded_at");

        builder.Property(r => r.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.Property(r => r.CancellationReason)
            .HasColumnName("cancellation_reason");

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(r => r.CustomerId)
            .HasDatabaseName("idx_reservations_customer");

        builder.HasIndex(r => r.ReservationDate)
            .HasDatabaseName("idx_reservations_date");

        builder.HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Table)
            .WithMany()
            .HasForeignKey(r => r.TableId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
