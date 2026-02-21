using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Orders.Commands.AssignDriver;

public class AssignDriverCommandHandler : IRequestHandler<AssignDriverCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public AssignDriverCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(AssignDriverCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure("Order not found");
        }

        var staff = await _context.Staff
            .FirstOrDefaultAsync(s => s.Id == request.StaffId && s.IsActive, cancellationToken);

        if (staff == null)
        {
            return Result.Failure("Staff not found or inactive");
        }

        order.AssignedDriverId = staff.Id;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
