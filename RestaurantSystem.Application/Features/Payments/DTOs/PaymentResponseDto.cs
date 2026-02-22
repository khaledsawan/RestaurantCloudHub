using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Payments.DTOs;

public class PaymentResponseDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
}
