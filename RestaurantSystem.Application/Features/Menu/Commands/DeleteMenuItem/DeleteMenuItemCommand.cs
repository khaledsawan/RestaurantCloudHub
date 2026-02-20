using MediatR;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Menu.Commands.DeleteMenuItem;

public record DeleteMenuItemCommand(int Id) : IRequest<Result>;
