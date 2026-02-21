using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Orders.Commands.AddOrderNote;

public record AddOrderNoteCommand : IRequest<Result>
{
    public int OrderId { get; init; }
    public OrderNoteType NoteType { get; init; }
    public string Note { get; init; } = string.Empty;
}
