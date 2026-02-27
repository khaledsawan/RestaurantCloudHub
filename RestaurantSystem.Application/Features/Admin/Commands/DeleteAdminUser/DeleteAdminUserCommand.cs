using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Admin.Commands.DeleteAdminUser;

public record DeleteAdminUserCommand(int UserId) : IRequest<Result>;
