using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("order_id");

        builder.Property(o => o.OrderNumber)
            .HasColumnName("order_number")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.CustomerId)
            .HasColumnName("customer_id");

        builder.Property(o => o.StaffId)
            .HasColumnName("staff_id");

        builder.Property(o => o.AssignedChefId)
            .HasColumnName("assigned_chef_id");

        builder.Property(o => o.AssignedDriverId)
            .HasColumnName("assigned_driver_id");

        builder.Property(o => o.OrderType)
            .HasColumnName("order_type")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<OrderType>(v, true));

        builder.Property(o => o.OrderStatus)
            .HasColumnName("order_status")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLowerInvariant(),
                v => Enum.Parse<OrderStatus>(v, true));

        builder.Property(o => o.Subtotal)
            .HasColumnName("subtotal")
            .HasPrecision(10, 2);

        builder.Property(o => o.TaxRate)
            .HasColumnName("tax_rate")
            .HasPrecision(5, 4);

        builder.Property(o => o.TaxAmount)
            .HasColumnName("tax_amount")
            .HasPrecision(10, 2);

        builder.Property(o => o.DeliveryFee)
            .HasColumnName("delivery_fee")
            .HasPrecision(10, 2);

        builder.Property(o => o.DiscountAmount)
            .HasColumnName("discount_amount")
            .HasPrecision(10, 2);

        builder.Property(o => o.DiscountCode)
            .HasColumnName("discount_code")
            .HasMaxLength(50);

        builder.Property(o => o.LoyaltyPointsUsed)
            .HasColumnName("loyalty_points_used");

        builder.Property(o => o.LoyaltyPointsDiscount)
            .HasColumnName("loyalty_points_discount")
            .HasPrecision(10, 2);

        builder.Property(o => o.TipAmount)
            .HasColumnName("tip_amount")
            .HasPrecision(10, 2);

        builder.Property(o => o.TotalAmount)
            .HasColumnName("total_amount")
            .HasPrecision(10, 2);

        builder.Property(o => o.EstimatedReadyTime)
            .HasColumnName("estimated_ready_time");

        builder.Property(o => o.ActualReadyTime)
            .HasColumnName("actual_ready_time");

        builder.Property(o => o.EstimatedDeliveryTime)
            .HasColumnName("estimated_delivery_time");

        builder.Property(o => o.ActualDeliveryTime)
            .HasColumnName("actual_delivery_time");

        builder.Property(o => o.CustomerNotes)
            .HasColumnName("customer_notes");

        builder.Property(o => o.KitchenNotes)
            .HasColumnName("kitchen_notes");

        builder.Property(o => o.DeliveryNotes)
            .HasColumnName("delivery_notes");

        builder.Property(o => o.DeliveryAddressId)
            .HasColumnName("delivery_address_id");

        builder.Property(o => o.DeliveryLatitude)
            .HasColumnName("delivery_latitude")
            .HasPrecision(10, 8);

        builder.Property(o => o.DeliveryLongitude)
            .HasColumnName("delivery_longitude")
            .HasPrecision(11, 8);

        builder.Property(o => o.CustomerRating)
            .HasColumnName("customer_rating");

        builder.Property(o => o.CustomerFeedback)
            .HasColumnName("customer_feedback");

        builder.Property(o => o.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.Property(o => o.CancellationReason)
            .HasColumnName("cancellation_reason");

        builder.Property(o => o.CancelledByType)
            .HasColumnName("cancelled_by_type")
            .HasMaxLength(20)
            .HasConversion(
                v => v.HasValue ? v.Value.ToString().ToLowerInvariant() : null,
                v => string.IsNullOrWhiteSpace(v) ? null : Enum.Parse<CancelledByType>(v, true));

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(o => o.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasIndex(o => o.CustomerId)
            .HasDatabaseName("idx_orders_customer");

        builder.HasIndex(o => o.OrderStatus)
            .HasDatabaseName("idx_orders_status");

        builder.HasIndex(o => o.OrderType)
            .HasDatabaseName("idx_orders_type");

        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("idx_orders_created");

        builder.HasIndex(o => o.OrderNumber)
            .HasDatabaseName("idx_orders_number");

        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.Staff)
            .WithMany()
            .HasForeignKey(o => o.StaffId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.AssignedChef)
            .WithMany()
            .HasForeignKey(o => o.AssignedChefId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.AssignedDriver)
            .WithMany()
            .HasForeignKey(o => o.AssignedDriverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.DeliveryAddress)
            .WithMany()
            .HasForeignKey(o => o.DeliveryAddressId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
