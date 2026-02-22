using RestaurantSystem.Domain.Common;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities;

public class Reservation : BaseEntity
{
    public int CustomerId { get; set; }
    public int? TableId { get; set; }
    public DateOnly ReservationDate { get; set; }
    public TimeOnly ReservationTime { get; set; }
    public int PartySize { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? SpecialRequests { get; set; }
    public string? CustomerNotes { get; set; }
    public string? StaffNotes { get; set; }
    public string? ConfirmationCode { get; set; }
    public DateTime? RemindedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual RestaurantTable? Table { get; set; }
}
