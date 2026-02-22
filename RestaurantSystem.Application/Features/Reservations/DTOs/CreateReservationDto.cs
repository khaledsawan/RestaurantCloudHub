using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Features.Reservations.DTOs;

public class CreateReservationDto
{
    public int TableId { get; set; }
    public DateOnly ReservationDate { get; set; }
    public TimeOnly ReservationTime { get; set; }
    public int PartySize { get; set; }
    public string? SpecialRequests { get; set; }
    public string? CustomerNotes { get; set; }
}
