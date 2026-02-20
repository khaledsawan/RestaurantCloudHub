using MediatR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.SetUserActiveStatus;

public class SetUserActiveStatusCommandHandler : IRequestHandler<SetUserActiveStatusCommand, Result>
{
    private readonly IIdentityService _identityService;

    public SetUserActiveStatusCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result> Handle(SetUserActiveStatusCommand request, CancellationToken cancellationToken)
    {
        return _identityService.SetUserActiveStatusAsync(request.UserId, request.IsActive);
    }
}
