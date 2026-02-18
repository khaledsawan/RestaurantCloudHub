using MediatR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return _identityService.ResetPasswordAsync(request.Email, request.Code, request.NewPassword);
    }
}
