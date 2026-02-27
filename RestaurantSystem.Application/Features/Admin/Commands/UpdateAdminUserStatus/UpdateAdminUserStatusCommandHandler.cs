using MediatR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;

namespace RestaurantSystem.Application.Features.Admin.Commands.UpdateAdminUserStatus;

public class UpdateAdminUserStatusCommandHandler : IRequestHandler<UpdateAdminUserStatusCommand, Result>
{
    private readonly IIdentityService _identityService;

    public UpdateAdminUserStatusCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<Result> Handle(UpdateAdminUserStatusCommand request, CancellationToken cancellationToken)
    {
        return _identityService.SetUserActiveStatusAsync(request.UserId, request.IsActive);
    }
}
