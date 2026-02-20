using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.DeleteAccount;

public record DeleteAccountCommand : IRequest<Result>;
