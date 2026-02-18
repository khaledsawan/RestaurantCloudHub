using MediatR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Auth.DTOs;

namespace RestaurantSystem.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public GetCurrentUserQueryHandler(
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return new CurrentUserDto { IsAuthenticated = false };
        }

        var result = await _identityService.GetUserByIdAsync(_currentUserService.UserId.Value);

        if (!result.Succeeded || result.Data == null)
        {
            return new CurrentUserDto { IsAuthenticated = false };
        }

        return new CurrentUserDto
        {
            IsAuthenticated = true,
            UserId = result.Data.Id,
            Email = result.Data.Email,
            FirstName = result.Data.FirstName,
            LastName = result.Data.LastName,
            EmailConfirmed = result.Data.EmailConfirmed,
            Roles = result.Data.Roles
        };
    }
}
