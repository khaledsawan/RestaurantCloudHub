using MediatR;
using RestaurantSystem.Application.Features.Customers.DTOs;

namespace RestaurantSystem.Application.Features.Customers.Queries.GetCustomerProfile;

public record GetCustomerProfileQuery : IRequest<CustomerProfileDto?>;
