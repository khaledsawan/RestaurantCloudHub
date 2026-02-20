using MediatR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.ChangeEmail;

public class RequestEmailChangeCommandHandler : IRequestHandler<RequestEmailChangeCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public RequestEmailChangeCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public Task<Result> Handle(RequestEmailChangeCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Task.FromResult(Result.Failure("User not authenticated"));
        }

        return _identityService.RequestEmailChangeAsync(_currentUserService.UserId.Value, request.NewEmail);
    }
}
