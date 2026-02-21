using RestaurantSystem.Application.Features.Orders.Commands.AddOrderNote;

namespace RestaurantSystem.WebAPI.Models;

public class AddOrderNoteRequest
{
    public OrderNoteType NoteType { get; set; }
    public string Note { get; set; } = string.Empty;
}
