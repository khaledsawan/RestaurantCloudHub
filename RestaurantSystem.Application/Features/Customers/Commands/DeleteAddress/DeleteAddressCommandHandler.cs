using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Customers.Commands.DeleteAddress;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteAddressCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Result.Failure("User not authenticated");
        }

        var address = await _context.CustomerAddresses
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.Id == request.AddressId, cancellationToken);

        if (address == null || address.Customer.UserId != _currentUserService.UserId.Value)
        {
            return Result.Failure("Address not found");
        }

        _context.CustomerAddresses.Remove(address);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
