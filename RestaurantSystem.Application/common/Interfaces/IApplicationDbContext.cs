using Microsoft.EntityFrameworkCore;
using RestaurantSystem.Domain.Entities;

namespace RestaurantSystem.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<CustomerAddress> CustomerAddresses { get; }
    DbSet<Category> Categories { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<MenuItemOptionGroup> MenuItemOptionGroups { get; }
    DbSet<MenuItemOption> MenuItemOptions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
