using MediatR;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.Auth.DTOs;

namespace RestaurantSystem.Application.Features.Auth.Queries.ValidateToken;

public class ValidateTokenQueryHandler : IRequestHandler<ValidateTokenQuery, TokenValidationDto>
{
    private readonly ITokenService _tokenService;

    public ValidateTokenQueryHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<TokenValidationDto> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        var userId = _tokenService.ValidateToken(request.Token);
        return Task.FromResult(new TokenValidationDto
        {
            IsValid = userId.HasValue,
            UserId = userId
        });
    }
}
