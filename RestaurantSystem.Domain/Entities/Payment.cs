using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public decimal Amount { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? TransactionId { get; set; }
    public string? Gateway { get; set; }
    public string? GatewayResponse { get; set; }
    public decimal RefundAmount { get; set; } = 0;
    public string? RefundReason { get; set; }
    public DateTime? RefundedAt { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    public virtual Order Order { get; set; } = null!;
}
