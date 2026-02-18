using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Domain.Entities.Identity;

namespace RestaurantSystem.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;

    public ApplicationDbContextInitializer(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<ApplicationDbContextInitializer> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedAdminAsync();
    }

    private async Task SeedAdminAsync()
    {
        var adminEmail = _configuration["Admin:Email"];
        var adminPassword = _configuration["Admin:Password"];
        var adminFirstName = _configuration["Admin:FirstName"];
        var adminLastName = _configuration["Admin:LastName"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword) ||
            string.IsNullOrWhiteSpace(adminFirstName) ||
            string.IsNullOrWhiteSpace(adminLastName))
        {
            _logger.LogInformation("Admin seeding skipped. Missing Admin configuration.");
            return;
        }

        var normalizedEmail = adminEmail.ToLowerInvariant();
        var existing = await _context.Set<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (existing != null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            Email = normalizedEmail,
            FirstName = adminFirstName,
            LastName = adminLastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            IsActive = true,
            EmailConfirmed = true
        };

        _context.Set<ApplicationUser>().Add(user);
        await _context.SaveChangesAsync();

        _context.Set<UserRole>().Add(new UserRole
        {
            UserId = user.Id,
            RoleName = "Admin"
        });

        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin user seeded: {Email}", adminEmail);
    }
}
