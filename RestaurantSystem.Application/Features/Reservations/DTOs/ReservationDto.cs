using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Reservations.DTOs;

public class ReservationDto
{
    public int Id { get; set; }
    public DateOnly ReservationDate { get; set; }
    public TimeOnly ReservationTime { get; set; }
    public int PartySize { get; set; }
    public ReservationStatus Status { get; set; }
    public string? ConfirmationCode { get; set; }
}
