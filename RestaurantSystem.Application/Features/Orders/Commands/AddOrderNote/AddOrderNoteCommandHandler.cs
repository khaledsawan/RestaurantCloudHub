using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Orders.Commands.AddOrderNote;

public class AddOrderNoteCommandHandler : IRequestHandler<AddOrderNoteCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AddOrderNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(AddOrderNoteCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure("Order not found");
        }

        var note = request.Note.Trim();

        switch (request.NoteType)
        {
            case OrderNoteType.Customer:
                order.CustomerNotes = Append(order.CustomerNotes, note);
                break;
            case OrderNoteType.Kitchen:
                order.KitchenNotes = Append(order.KitchenNotes, note);
                break;
            case OrderNoteType.Delivery:
                order.DeliveryNotes = Append(order.DeliveryNotes, note);
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static string Append(string? existing, string note)
    {
        if (string.IsNullOrWhiteSpace(existing))
        {
            return note;
        }

        return existing + Environment.NewLine + note;
    }
}
