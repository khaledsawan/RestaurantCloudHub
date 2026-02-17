using MediatR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Application.Features.Auth.Commands.Logout;

namespace RestaurantSystem.Application.Features.Auth.Commands.ConfirmEmail;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IIdentityService _identityService;

    public LogoutCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return _identityService.RevokeTokenAsync(request.RefreshToken);
    }
}
