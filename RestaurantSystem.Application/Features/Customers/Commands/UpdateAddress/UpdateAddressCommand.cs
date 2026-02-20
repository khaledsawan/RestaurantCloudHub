using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Customers.Commands.UpdateAddress;

public record UpdateAddressCommand : IRequest<Result>
{
    public int AddressId { get; init; }
    public string? Label { get; init; }
    public string AddressLine1 { get; init; } = string.Empty;
    public string? AddressLine2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string? State { get; init; }
    public string PostalCode { get; init; } = string.Empty;
    public string Country { get; init; } = "USA";
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public string? DeliveryInstructions { get; init; }
    public bool IsDefault { get; init; } = false;
}
