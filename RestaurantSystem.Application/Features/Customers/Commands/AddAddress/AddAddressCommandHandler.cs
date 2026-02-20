using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Features.Customers.Commands.AddAddress;

public class AddAddressCommandHandler : IRequestHandler<AddAddressCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AddAddressCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Result.Failure("User not authenticated");
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == _currentUserService.UserId.Value, cancellationToken);

        if (customer == null)
        {
            return Result.Failure("Customer profile not found");
        }

        var address = new CustomerAddress
        {
            CustomerId = customer.Id,
            Label = request.Label,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            DeliveryInstructions = request.DeliveryInstructions,
            IsDefault = request.IsDefault
        };

        _context.CustomerAddresses.Add(address);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
