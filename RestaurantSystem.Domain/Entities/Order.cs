using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public int? StaffId { get; set; }
    public int? AssignedChefId { get; set; }
    public int? AssignedDriverId { get; set; }

    public OrderType OrderType { get; set; } = OrderType.DineIn;
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public decimal Subtotal { get; set; } = 0;
    public decimal TaxRate { get; set; } = 0.08m;
    public decimal TaxAmount { get; set; } = 0;
    public decimal DeliveryFee { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public string? DiscountCode { get; set; }
    public int LoyaltyPointsUsed { get; set; } = 0;
    public decimal LoyaltyPointsDiscount { get; set; } = 0;
    public decimal TipAmount { get; set; } = 0;
    public decimal TotalAmount { get; set; } = 0;

    public DateTime? EstimatedReadyTime { get; set; }
    public DateTime? ActualReadyTime { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public DateTime? ActualDeliveryTime { get; set; }

    public string? CustomerNotes { get; set; }
    public string? KitchenNotes { get; set; }
    public string? DeliveryNotes { get; set; }

    public int? DeliveryAddressId { get; set; }
    public decimal? DeliveryLatitude { get; set; }
    public decimal? DeliveryLongitude { get; set; }

    public int? CustomerRating { get; set; }
    public string? CustomerFeedback { get; set; }

    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public CancelledByType? CancelledByType { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual Staff? Staff { get; set; }
    public virtual Staff? AssignedChef { get; set; }
    public virtual Staff? AssignedDriver { get; set; }
    public virtual CustomerAddress? DeliveryAddress { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
}
