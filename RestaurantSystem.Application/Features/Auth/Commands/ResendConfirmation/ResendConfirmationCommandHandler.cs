using MediatR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.ResendConfirmation;

public class ResendConfirmationCommandHandler : IRequestHandler<ResendConfirmationCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ResendConfirmationCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result> Handle(ResendConfirmationCommand request, CancellationToken cancellationToken)
    {
        return _identityService.ResendConfirmationAsync(request.Email);
    }
}
