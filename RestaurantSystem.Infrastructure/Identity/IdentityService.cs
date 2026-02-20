using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Common.Models;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Entities.Identity;
using BCrypt.Net;
using RestaurantSystem.Infrastructure.Persistence;
using RestaurantSystem.Application.Features.Auth.Commands.Logout;

namespace RestaurantSystem.Infrastructure.Identity;

/// <summary>
/// Identity service implementation
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IDateTime _dateTime;
    private readonly IEmailService _emailService;

    private readonly ICurrentUserService _currentUserService;

    public IdentityService(
        ApplicationDbContext context,
        ITokenService tokenService,
        IDateTime dateTime,
        IEmailService emailService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _tokenService = tokenService;
        _dateTime = dateTime;
        _emailService = emailService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> RegisterAsync(
        string email, 
        string password, 
        string firstName, 
        string lastName, 
        string role = "Customer")
    {
        // Check if email already exists
        if (await EmailExistsAsync(email))
        {
            return Result<int>.Failure("Email already registered");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        // Create user
        var user = new ApplicationUser
        {
            Email = email.ToLowerInvariant(),
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash,
            IsActive = true,
            EmailConfirmed = false
        };

        _context.Set<ApplicationUser>().Add(user);
        await _context.SaveChangesAsync();

        // Assign role
        var userRole = new Domain.Entities.Identity.UserRole
        {
            UserId = user.Id,
            RoleName = role
        };

        _context.Set<Domain.Entities.Identity.UserRole>().Add(userRole);
        await _context.SaveChangesAsync();

        await SendEmailConfirmationAsync(user);

        return Result<int>.Success(user.Id);
    }

    public async Task<Result<AuthResult>> LoginAsync(string email, string password)
    {
        // Find user
        var user = await _context.Set<ApplicationUser>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        if (user == null)
        {
            return Result<AuthResult>.Failure("Invalid email or password");
        }

        // Check if account is locked
        if (user.LockoutEnd.HasValue && user.LockoutEnd > _dateTime.UtcNow)
        {
            return Result<AuthResult>.Failure("Account is locked. Try again later.");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            // Increment failed login attempts
            user.FailedLoginAttempts++;
            
            // Lock account after 5 failed attempts
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = _dateTime.UtcNow.AddMinutes(30);
            }

            await _context.SaveChangesAsync();
            return Result<AuthResult>.Failure("Invalid email or password");
        }

        // Check if account is active
        if (!user.IsActive)
        {
            return Result<AuthResult>.Failure("Account is deactivated");
        }

        if (!user.EmailConfirmed)
        {
            return Result<AuthResult>.Failure("Email not confirmed. Please confirm your email before logging in.");
        }

        // Reset failed login attempts
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = _dateTime.UtcNow;

        // Get roles
        var roles = user.UserRoles.Select(r => r.RoleName).ToList();

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Store refresh token
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = _dateTime.UtcNow.AddDays(7)
        };

        _context.Set<RefreshToken>().Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        var authResult = new AuthResult
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _dateTime.UtcNow.AddMinutes(_tokenService.GetTokenExpirationMinutes()),
            Roles = roles
        };

        return Result<AuthResult>.Success(authResult);
    }

    public async Task<Result> ConfirmEmailAsync(string email, string token)
    {
        var user = await _context.Set<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure("Email already confirmed");
        }

        if (string.IsNullOrWhiteSpace(user.EmailConfirmationTokenHash) ||
            !user.EmailConfirmationTokenExpiresAt.HasValue ||
            user.EmailConfirmationTokenExpiresAt.Value < _dateTime.UtcNow)
        {
            return Result.Failure("Confirmation token is invalid or expired");
        }

        if (!VerifyTokenHash(token, user.EmailConfirmationTokenHash))
        {
            return Result.Failure("Confirmation token is invalid or expired");
        }

        user.EmailConfirmed = true;
        user.EmailConfirmationTokenHash = null;
        user.EmailConfirmationTokenExpiresAt = null;
        user.LastConfirmationSentAt = null;

        var customer = await _context.Set<Customer>()
            .FirstOrDefaultAsync(c => c.UserId == user.Id);
        if (customer != null)
        {
            customer.IsVerified = true;
        }

        await _context.SaveChangesAsync();

        await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);

        return Result.Success();
    }

    public async Task<Result> ResendConfirmationAsync(string email)
    {
        var user = await _context.Set<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure("Email already confirmed");
        }

        if (user.LastConfirmationSentAt.HasValue &&
            user.LastConfirmationSentAt.Value.AddMinutes(2) > _dateTime.UtcNow)
        {
            return Result.Failure("Please wait before requesting another confirmation email");
        }

        await SendEmailConfirmationAsync(user);

        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(string email)
    {
        var user = await _context.Set<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        if (user == null)
        {
            return Result.Success();
        }

        if (user.LastPasswordResetSentAt.HasValue &&
            user.LastPasswordResetSentAt.Value.AddMinutes(2) > _dateTime.UtcNow)
        {
            return Result.Success();
        }

        var code = GenerateEmailCode();
        user.PasswordResetTokenHash = HashToken(code);
        user.PasswordResetTokenExpiresAt = _dateTime.UtcNow.AddHours(24);
        user.LastPasswordResetSentAt = _dateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _emailService.SendPasswordResetCodeAsync(user.Email, code, user.FirstName);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string email, string code, string newPassword)
    {
        var user = await _context.Set<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        if (user == null)
        {
            return Result.Failure("Invalid email or code");
        }

        if (string.IsNullOrWhiteSpace(user.PasswordResetTokenHash) ||
            !user.PasswordResetTokenExpiresAt.HasValue ||
            user.PasswordResetTokenExpiresAt.Value < _dateTime.UtcNow)
        {
            return Result.Failure("Invalid or expired code");
        }

        if (!VerifyTokenHash(code, user.PasswordResetTokenHash))
        {
            return Result.Failure("Invalid or expired code");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpiresAt = null;
        user.LastPasswordResetSentAt = null;

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RequestEmailChangeAsync(int userId, string newEmail)
    {
        var user = await _context.Set<ApplicationUser>().FindAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        var normalized = newEmail.ToLowerInvariant();
        if (string.Equals(normalized, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure("New email must be different");
        }

        if (await EmailExistsAsync(normalized))
        {
            return Result.Failure("Email already in use");
        }

        if (user.LastEmailChangeSentAt.HasValue &&
            user.LastEmailChangeSentAt.Value.AddMinutes(2) > _dateTime.UtcNow)
        {
            return Result.Failure("Please wait before requesting another email change");
        }

        var code = GenerateEmailCode();
        user.PendingEmail = normalized;
        user.EmailChangeTokenHash = HashToken(code);
        user.EmailChangeTokenExpiresAt = _dateTime.UtcNow.AddHours(24);
        user.LastEmailChangeSentAt = _dateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _emailService.SendEmailChangeCodeAsync(normalized, code, user.FirstName);

        return Result.Success();
    }

    public async Task<Result> ConfirmEmailChangeAsync(int userId, string newEmail, string code)
    {
        var user = await _context.Set<ApplicationUser>().FindAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        var normalized = newEmail.ToLowerInvariant();
        if (!string.Equals(user.PendingEmail, normalized, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure("Email change not requested");
        }

        if (string.IsNullOrWhiteSpace(user.EmailChangeTokenHash) ||
            !user.EmailChangeTokenExpiresAt.HasValue ||
            user.EmailChangeTokenExpiresAt.Value < _dateTime.UtcNow)
        {
            return Result.Failure("Invalid or expired code");
        }

        if (!VerifyTokenHash(code, user.EmailChangeTokenHash))
        {
            return Result.Failure("Invalid or expired code");
        }

        user.Email = normalized;
        user.EmailConfirmed = true;
        user.PendingEmail = null;
        user.EmailChangeTokenHash = null;
        user.EmailChangeTokenExpiresAt = null;
        user.LastEmailChangeSentAt = null;

        var customer = await _context.Set<Customer>()
            .FirstOrDefaultAsync(c => c.UserId == user.Id);
        if (customer != null)
        {
            customer.Email = normalized;
            customer.IsVerified = true;
        }

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAccountAsync(int userId)
    {
        var user = await _context.Set<ApplicationUser>().FindAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        user.IsActive = false;

        var refreshTokens = await _context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in refreshTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = _dateTime.UtcNow;
        }

        var customer = await _context.Set<Customer>()
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (customer != null)
        {
            customer.IsActive = false;
            customer.DeletedAt = _dateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> SetUserActiveStatusAsync(int userId, bool isActive)
    {
        var user = await _context.Set<ApplicationUser>().FindAsync(userId);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        user.IsActive = isActive;

        var customer = await _context.Set<Customer>()
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (customer != null)
        {
            customer.IsActive = isActive;
        }

        if (!isActive)
        {
            var refreshTokens = await _context.Set<RefreshToken>()
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = _dateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<AuthResult>> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.Set<RefreshToken>()
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || !storedToken.IsActive)
        {
            return Result<AuthResult>.Failure("Invalid or expired refresh token");
        }

        var user = storedToken.User;

        // Revoke old token
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = _dateTime.UtcNow;

        // Generate new tokens
        var roles = user.UserRoles.Select(r => r.RoleName).ToList();
        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Store new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = _dateTime.UtcNow.AddDays(7)
        };

        storedToken.ReplacedByToken = newRefreshToken;
        _context.Set<RefreshToken>().Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        var authResult = new AuthResult
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = _dateTime.UtcNow.AddMinutes(_tokenService.GetTokenExpirationMinutes()),
            Roles = roles
        };

        return Result<AuthResult>.Success(authResult);
    }

    public async Task<Result> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Set<ApplicationUser>().FindAsync(userId);

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
        {
            return Result.Failure("Current password is incorrect");
        }

        // Hash and set new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(int userId)
    {
        var user = await _context.Set<ApplicationUser>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.UserRoles.Select(r => r.RoleName),
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt
        };

        return Result<UserDto>.Success(userDto);
    }

    public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
    {
        var user = await _context.Set<ApplicationUser>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.UserRoles.Select(r => r.RoleName),
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt
        };

        return Result<UserDto>.Success(userDto);
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        return await _context.Set<ApplicationUser>()
            .AnyAsync(u => u.Id == userId);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Set<ApplicationUser>()
            .AnyAsync(u => u.Email == email.ToLowerInvariant());
    }

    public async Task<Result> RevokeTokenAsync(string refreshToken)
    {

        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return Result.Failure("User not authenticated");
        }

        if(string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Failure("Refresh token is required");
        }
        
        var token = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (token == null)
        {
            return Result.Failure("Token not found");
        }

        token.IsRevoked = true;
        token.RevokedAt = _dateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> AssignRoleAsync(int userId, string role)
    {
        var user = await _context.Set<ApplicationUser>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Result.Failure("User not found");
        }

        if (user.UserRoles.Any(r => r.RoleName == role))
        {
            return Result.Failure("User already has this role");
        }

        var userRole = new Domain.Entities.Identity.UserRole
        {
            UserId = userId,
            RoleName = role
        };

        _context.Set<Domain.Entities.Identity.UserRole>().Add(userRole);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RemoveRoleAsync(int userId, string role)
    {
        var userRole = await _context.Set<Domain.Entities.Identity.UserRole>()
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleName == role);

        if (userRole == null)
        {
            return Result.Failure("User does not have this role");
        }

        _context.Set<Domain.Entities.Identity.UserRole>().Remove(userRole);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    private async Task SendEmailConfirmationAsync(ApplicationUser user)
    {
        var code = GenerateEmailCode();
        user.EmailConfirmationTokenHash = HashToken(code);
        user.EmailConfirmationTokenExpiresAt = _dateTime.UtcNow.AddHours(24);
        user.LastConfirmationSentAt = _dateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _emailService.SendEmailConfirmationAsync(user.Email, code, user.FirstName);
    }

    private static string GenerateEmailCode()
    {
        var bytes = new byte[4];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        var value = BitConverter.ToUInt32(bytes, 0) % 1_000_000;
        return value.ToString("D6");
    }

    private static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    private static bool VerifyTokenHash(string token, string storedHash)
    {
        var tokenHash = HashToken(token);
        var a = Encoding.UTF8.GetBytes(tokenHash);
        var b = Encoding.UTF8.GetBytes(storedHash);
        return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
    }
}
