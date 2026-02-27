using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Entities.Identity;

namespace RestaurantSystem.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<CustomerAddress> CustomerAddresses { get; }
    DbSet<Staff> Staff { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderStatusHistory> OrderStatusHistories { get; }
    DbSet<OrderItemOption> OrderItemOptions { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<RestaurantTable> RestaurantTables { get; }
    DbSet<InventoryCategory> InventoryCategories { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<InventoryTransaction> InventoryTransactions { get; }
    DbSet<MenuItemIngredient> MenuItemIngredients { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Category> Categories { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<MenuItemOptionGroup> MenuItemOptionGroups { get; }
    DbSet<MenuItemOption> MenuItemOptions { get; }
    DbSet<ApplicationUser> Users { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<SystemSetting> SystemSettings { get; }
    DbSet<RestaurantSetting> RestaurantSettings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
