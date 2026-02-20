using MediatR;
using RestaurantSystem.Application.Features.Customers.DTOs;

namespace RestaurantSystem.Application.Features.Customers.Queries.GetCustomerAddresses;

public record GetCustomerAddressesQuery : IRequest<List<CustomerAddressDto>>;
