using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("payment_id");

        builder.Property(p => p.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(p => p.PaymentMethod)
            .HasColumnName("payment_method")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<PaymentMethod>(v, true));

        builder.Property(p => p.Amount)
            .HasColumnName("amount")
            .HasPrecision(10, 2);

        builder.Property(p => p.PaymentStatus)
            .HasColumnName("payment_status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<PaymentStatus>(v, true));

        builder.Property(p => p.TransactionId)
            .HasColumnName("transaction_id")
            .HasMaxLength(255);

        builder.Property(p => p.Gateway)
            .HasColumnName("gateway")
            .HasMaxLength(50);

        builder.Property(p => p.GatewayResponse)
            .HasColumnName("gateway_response");

        builder.Property(p => p.RefundAmount)
            .HasColumnName("refund_amount")
            .HasPrecision(10, 2);

        builder.Property(p => p.RefundReason)
            .HasColumnName("refund_reason");

        builder.Property(p => p.RefundedAt)
            .HasColumnName("refunded_at");

        builder.Property(p => p.PaymentDate)
            .HasColumnName("payment_date");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at");

        builder.Ignore(p => p.UpdatedAt);

        builder.HasIndex(p => p.OrderId)
            .HasDatabaseName("idx_payments_order");

        builder.HasIndex(p => p.PaymentStatus)
            .HasDatabaseName("idx_payments_status");

        builder.HasIndex(p => p.TransactionId)
            .HasDatabaseName("idx_payments_transaction");

        builder.HasOne(p => p.Order)
            .WithMany()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
