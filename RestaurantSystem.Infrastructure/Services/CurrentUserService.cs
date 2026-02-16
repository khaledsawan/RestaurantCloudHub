using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RestaurantSystem.Application.Common.Interfaces;

namespace RestaurantSystem.Infrastructure.Services;

/// <summary>
/// Service for accessing current authenticated user from HTTP context
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public IEnumerable<string> Roles
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User
                ?.FindAll(ClaimTypes.Role)
                ?.Select(c => c.Value) ?? Enumerable.Empty<string>();
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }

    public bool IsInAnyRole(params string[] roles)
    {
        return roles.Any(role => IsInRole(role));
    }
}