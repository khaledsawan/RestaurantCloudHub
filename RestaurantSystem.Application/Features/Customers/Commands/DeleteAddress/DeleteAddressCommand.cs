using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Customers.Commands.DeleteAddress;

public record DeleteAddressCommand(int AddressId) : IRequest<Result>;
