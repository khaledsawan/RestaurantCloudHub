using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Entities.Identity;

namespace RestaurantSystem.Infrastructure.Persistence;

/// <summary>
/// Application database context
/// Simple, direct, practical approach
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ========================================
    // DbSets - Add as you create entities
    // ========================================
    
    // Auth
    public DbSet<ApplicationUser> Users { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    // Customers
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<CustomerAddress> CustomerAddresses { get; set; } = null!;

    // Orders
    public DbSet<Staff> Staff { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
    public DbSet<OrderItemOption> OrderItemOptions { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<RestaurantTable> RestaurantTables { get; set; } = null!;

    // Menu
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<MenuItemOptionGroup> MenuItemOptionGroups { get; set; } = null!;
    public DbSet<MenuItemOption> MenuItemOptions { get; set; } = null!;

    // ========================================
    // Configuration
    // ========================================
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all entity configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    // ========================================
    // Optional: Override SaveChanges for interceptors
    // ========================================
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // You can add custom logic here if needed
        // For example, automatically set UpdatedAt timestamps
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}
