using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Customers.Commands.UpdateAddress;

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAddressCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
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

        address.Label = request.Label;
        address.AddressLine1 = request.AddressLine1;
        address.AddressLine2 = request.AddressLine2;
        address.City = request.City;
        address.State = request.State;
        address.PostalCode = request.PostalCode;
        address.Country = request.Country;
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;
        address.DeliveryInstructions = request.DeliveryInstructions;
        address.IsDefault = request.IsDefault;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
