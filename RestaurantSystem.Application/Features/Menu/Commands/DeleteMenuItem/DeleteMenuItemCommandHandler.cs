using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.DeleteMenuItem;

public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteMenuItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (item == null)
        {
            return Result.Failure("Menu item not found");
        }

        item.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
