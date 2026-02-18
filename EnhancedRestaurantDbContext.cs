// =====================================================
// ENHANCED DATABASE CONTEXT - PRODUCTION READY
// =====================================================

using Microsoft.EntityFrameworkCore;
using RestaurantApp.Models;

namespace RestaurantApp.Data
{
    public class EnhancedRestaurantDbContext : DbContext
    {
        public EnhancedRestaurantDbContext(DbContextOptions<EnhancedRestaurantDbContext> options)
            : base(options)
        {
        }

        // All DbSets
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemOptionGroup> MenuItemOptionGroups { get; set; }
        public DbSet<MenuItemOption> MenuItemOptions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<CustomerPaymentMethod> CustomerPaymentMethods { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<RestaurantTable> RestaurantTables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemOption> OrderItemOptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<InventoryCategory> InventoryCategories { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<MenuItemIngredient> MenuItemIngredients { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<DiscountCodeUsage> DiscountCodeUsages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureDecimalPrecision(modelBuilder);
            ConfigureIndexes(modelBuilder);
            ConfigureRelationships(modelBuilder);
            ConfigureQueryFilters(modelBuilder);
            SeedData(modelBuilder);
        }

        private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            // MenuItem
            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasPrecision(10, 2);

            // MenuItemOption
            modelBuilder.Entity<MenuItemOption>()
                .Property(o => o.PriceAdjustment)
                .HasPrecision(10, 2);

            // Customer
            modelBuilder.Entity<Customer>()
                .Property(c => c.TotalSpent)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Customer>()
                .Property(c => c.AverageRating)
                .HasPrecision(3, 2);

            // CustomerAddress coordinates
            modelBuilder.Entity<CustomerAddress>()
                .Property(ca => ca.Latitude)
                .HasPrecision(10, 8);

            modelBuilder.Entity<CustomerAddress>()
                .Property(ca => ca.Longitude)
                .HasPrecision(11, 8);

            // Staff
            modelBuilder.Entity<Staff>()
                .Property(s => s.HourlyRate)
                .HasPrecision(10, 2);

            // Order - all decimal fields
            modelBuilder.Entity<Order>()
                .Property(o => o.Subtotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TaxRate)
                .HasPrecision(5, 4);

            modelBuilder.Entity<Order>()
                .Property(o => o.TaxAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryFee)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.DiscountAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.LoyaltyPointsDiscount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TipAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryLatitude)
                .HasPrecision(10, 8);

            modelBuilder.Entity<Order>()
                .Property(o => o.DeliveryLongitude)
                .HasPrecision(11, 8);

            // OrderItem
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Subtotal)
                .HasPrecision(10, 2);

            // OrderItemOption
            modelBuilder.Entity<OrderItemOption>()
                .Property(oio => oio.PriceAdjustment)
                .HasPrecision(10, 2);

            // Payment
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.RefundAmount)
                .HasPrecision(10, 2);

            // InventoryItem
            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.CurrentQuantity)
                .HasPrecision(10, 2);

            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.MinimumQuantity)
                .HasPrecision(10, 2);

            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.ReorderQuantity)
                .HasPrecision(10, 2);

            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.UnitCost)
                .HasPrecision(10, 2);

            // InventoryTransaction
            modelBuilder.Entity<InventoryTransaction>()
                .Property(it => it.QuantityChange)
                .HasPrecision(10, 2);

            modelBuilder.Entity<InventoryTransaction>()
                .Property(it => it.QuantityAfter)
                .HasPrecision(10, 2);

            modelBuilder.Entity<InventoryTransaction>()
                .Property(it => it.UnitCost)
                .HasPrecision(10, 2);

            // MenuItemIngredient
            modelBuilder.Entity<MenuItemIngredient>()
                .Property(mii => mii.QuantityRequired)
                .HasPrecision(10, 3);

            // DiscountCode
            modelBuilder.Entity<DiscountCode>()
                .Property(dc => dc.DiscountValue)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DiscountCode>()
                .Property(dc => dc.MinimumOrderAmount)
                .HasPrecision(10, 2);

            // DiscountCodeUsage
            modelBuilder.Entity<DiscountCodeUsage>()
                .Property(dcu => dcu.DiscountAmount)
                .HasPrecision(10, 2);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Unique constraints
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<RestaurantTable>()
                .HasIndex(t => t.TableNumber)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<InventoryItem>()
                .HasIndex(i => i.Sku)
                .IsUnique();

            modelBuilder.Entity<DiscountCode>()
                .HasIndex(dc => dc.Code)
                .IsUnique();

            modelBuilder.Entity<Reservation>()
                .HasIndex(r => r.ConfirmationCode)
                .IsUnique();

            modelBuilder.Entity<MenuItemIngredient>()
                .HasIndex(mii => new { mii.ItemId, mii.InventoryItemId })
                .IsUnique();
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Menu relationships
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItemOptionGroup>()
                .HasOne(og => og.MenuItem)
                .WithMany(m => m.OptionGroups)
                .HasForeignKey(og => og.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItemOption>()
                .HasOne(o => o.OptionGroup)
                .WithMany(og => og.Options)
                .HasForeignKey(o => o.OptionGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer relationships
            modelBuilder.Entity<CustomerAddress>()
                .HasOne(ca => ca.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(ca => ca.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerPaymentMethod>()
                .HasOne(pm => pm.Customer)
                .WithMany(c => c.PaymentMethods)
                .HasForeignKey(pm => pm.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Staff relationships
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Role)
                .WithMany(r => r.StaffMembers)
                .HasForeignKey(s => s.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reservation relationships
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Table)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.SetNull);

            // Order relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Staff)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.AssignedChef)
                .WithMany(s => s.OrdersAsChef)
                .HasForeignKey(o => o.AssignedChefId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.AssignedDriver)
                .WithMany(s => s.OrdersAsDriver)
                .HasForeignKey(o => o.AssignedDriverId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.DeliveryAddress)
                .WithMany()
                .HasForeignKey(o => o.DeliveryAddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // Order status history
            modelBuilder.Entity<OrderStatusHistory>()
                .HasOne(osh => osh.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(osh => osh.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order items
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order item options
            modelBuilder.Entity<OrderItemOption>()
                .HasOne(oio => oio.OrderItem)
                .WithMany(oi => oi.SelectedOptions)
                .HasForeignKey(oio => oio.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItemOption>()
                .HasOne(oio => oio.Option)
                .WithMany(o => o.OrderItemOptions)
                .HasForeignKey(oio => oio.OptionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Inventory
            modelBuilder.Entity<InventoryItem>()
                .HasOne(ii => ii.InventoryCategory)
                .WithMany(ic => ic.InventoryItems)
                .HasForeignKey(ii => ii.InventoryCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.InventoryItem)
                .WithMany(ii => ii.Transactions)
                .HasForeignKey(it => it.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Staff)
                .WithMany()
                .HasForeignKey(it => it.StaffId)
                .OnDelete(DeleteBehavior.SetNull);

            // Menu item ingredients
            modelBuilder.Entity<MenuItemIngredient>()
                .HasOne(mii => mii.MenuItem)
                .WithMany(m => m.Ingredients)
                .HasForeignKey(mii => mii.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItemIngredient>()
                .HasOne(mii => mii.InventoryItem)
                .WithMany(ii => ii.MenuItemIngredients)
                .HasForeignKey(mii => mii.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Discount codes
            modelBuilder.Entity<DiscountCodeUsage>()
                .HasOne(dcu => dcu.DiscountCode)
                .WithMany(dc => dc.Usages)
                .HasForeignKey(dcu => dcu.DiscountCodeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiscountCodeUsage>()
                .HasOne(dcu => dcu.Customer)
                .WithMany()
                .HasForeignKey(dcu => dcu.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DiscountCodeUsage>()
                .HasOne(dcu => dcu.Order)
                .WithMany()
                .HasForeignKey(dcu => dcu.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Order)
                .WithMany(o => o.Reviews)
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.RespondedBy)
                .WithMany()
                .HasForeignKey(r => r.RespondedById)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void ConfigureQueryFilters(ModelBuilder modelBuilder)
        {
            // Global query filters for soft deletes
            modelBuilder.Entity<Category>()
                .HasQueryFilter(c => c.DeletedAt == null);

            modelBuilder.Entity<MenuItem>()
                .HasQueryFilter(m => m.DeletedAt == null);

            modelBuilder.Entity<Customer>()
                .HasQueryFilter(c => c.DeletedAt == null);

            modelBuilder.Entity<Staff>()
                .HasQueryFilter(s => s.DeletedAt == null);

            modelBuilder.Entity<InventoryItem>()
                .HasQueryFilter(i => i.DeletedAt == null);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin", Description = "Full system access", Permissions = "[\"all\"]" },
                new Role { RoleId = 2, RoleName = "Manager", Description = "Restaurant management", Permissions = "[\"orders.manage\",\"menu.manage\",\"staff.view\",\"reports.view\"]" },
                new Role { RoleId = 3, RoleName = "Waiter", Description = "Order taking and tables", Permissions = "[\"orders.create\",\"orders.view\",\"tables.manage\"]" },
                new Role { RoleId = 4, RoleName = "Chef", Description = "Kitchen operations", Permissions = "[\"orders.view\",\"orders.update_status\",\"inventory.view\"]" },
                new Role { RoleId = 5, RoleName = "Cashier", Description = "Payment processing", Permissions = "[\"orders.view\",\"payments.process\"]" },
                new Role { RoleId = 6, RoleName = "Delivery", Description = "Delivery driver", Permissions = "[\"orders.view\",\"orders.deliver\"]" }
            );

            // Seed default admin staff (replace password hash in real systems)
            modelBuilder.Entity<Staff>().HasData(
                new Staff
                {
                    StaffId = 1,
                    RoleId = 1,
                    Email = "admin@restaurant.com",
                    PasswordHash = "CHANGE_ME_HASH",
                    FirstName = "Admin",
                    LastName = "User",
                    HireDate = new DateTime(2024, 1, 1),
                    EmploymentStatus = "active",
                    IsActive = true
                }
            );
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}
