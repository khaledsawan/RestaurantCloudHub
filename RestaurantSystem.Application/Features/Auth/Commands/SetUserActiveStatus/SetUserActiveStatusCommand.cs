using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.SetUserActiveStatus;

public record SetUserActiveStatusCommand(int UserId, bool IsActive) : IRequest<Result>;
