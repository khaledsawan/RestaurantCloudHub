using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Entities.Identity;
using UserRoleEntity = RestaurantSystem.Domain.Entities.Identity.UserRole;
using RestaurantSystem.Domain.Enums;

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
        await SeedDummyDataAsync();
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

        _context.Set<UserRoleEntity>().Add(new UserRoleEntity
        {
            UserId = user.Id,
            RoleName = "Admin"
        });

        await _context.SaveChangesAsync();

        _logger.LogInformation("Admin user seeded: {Email}", adminEmail);
    }

    private async Task SeedDummyDataAsync()
    {
        var settings = SeedSettings.FromConfiguration(_configuration);
        if (!settings.Enabled)
        {
            return;
        }

        var marker = settings.Marker;
        var markerUpper = marker.ToUpperInvariant();
        var random = new Random();

        if (!await _context.Users.AnyAsync(u => EF.Functions.ILike(u.Email, marker + "+%")))
        {
            await SeedUsersAsync(settings, marker, random);
        }

        if (!await _context.Customers.IgnoreQueryFilters().AnyAsync(c => EF.Functions.ILike(c.Email, marker + "+%")))
        {
            await SeedCustomersAsync(settings, random);
        }

        if (!await _context.Staff.IgnoreQueryFilters().AnyAsync(s => EF.Functions.ILike(s.User.Email, marker + "+%")))
        {
            await SeedStaffAsync(settings, random);
        }

        if (!await _context.Categories.IgnoreQueryFilters().AnyAsync(c => EF.Functions.ILike(c.Name, marker + "%")))
        {
            await SeedMenuAsync(settings, marker, random);
        }

        if (!await _context.InventoryItems.IgnoreQueryFilters().AnyAsync(i => i.Sku != null && EF.Functions.ILike(i.Sku, markerUpper + "-SKU-%")))
        {
            await SeedInventoryAsync(settings, markerUpper, random);
        }

        if (!await _context.RestaurantTables.IgnoreQueryFilters().AnyAsync(t => EF.Functions.ILike(t.TableNumber, markerUpper + "-T-%")))
        {
            await SeedTablesAsync(settings, markerUpper);
        }

        if (!await _context.Orders.IgnoreQueryFilters().AnyAsync(o => EF.Functions.ILike(o.OrderNumber, markerUpper + "-ORD-%")))
        {
            await SeedOrdersAsync(settings, markerUpper, random);
        }

        var hasSeedPayments = await _context.Payments
            .IgnoreQueryFilters()
            .Join(_context.Orders.IgnoreQueryFilters(),
                p => p.OrderId,
                o => o.Id,
                (p, o) => o.OrderNumber)
            .AnyAsync(n => n.StartsWith(markerUpper + "-ORD-"));

        if (!hasSeedPayments)
        {
            await SeedPaymentsAsync(settings, random);
        }

        if (!await _context.Reservations.IgnoreQueryFilters().AnyAsync(r => r.ConfirmationCode != null && EF.Functions.ILike(r.ConfirmationCode, markerUpper + "-R-%")))
        {
            await SeedReservationsAsync(settings, markerUpper, random);
        }

        if (!await _context.Reviews.IgnoreQueryFilters().AnyAsync(r => r.ReviewText != null && EF.Functions.ILike(r.ReviewText, marker + "%")))
        {
            await SeedReviewsAsync(settings, marker, random);
        }
    }

    private async Task SeedUsersAsync(SeedSettings settings, string marker, Random random)
    {
        var names = SeedData.Names;
        var users = new List<ApplicationUser>();
        var roles = new List<UserRoleEntity>();

        for (var i = 0; i < settings.Users; i++)
        {
            var first = names[random.Next(names.Length)];
            var last = names[random.Next(names.Length)];
            var email = $"{marker}+{Guid.NewGuid():N}@demo.local";

            var user = new ApplicationUser
            {
                Email = email,
                FirstName = first,
                LastName = last,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Pass@123"),
                IsActive = true,
                EmailConfirmed = true
            };

            users.Add(user);
        }

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        var userIds = users.Select(u => u.Id).ToList();
        var customerCount = Math.Min(settings.Customers, userIds.Count);
        var staffCount = Math.Min(settings.Staff, Math.Max(0, userIds.Count - customerCount));

        var shuffled = userIds.OrderBy(_ => random.Next()).ToList();
        var customerUsers = shuffled.Take(customerCount).ToList();
        var staffUsers = shuffled.Skip(customerCount).Take(staffCount).ToList();
        var remainingUsers = shuffled.Skip(customerCount + staffCount).ToList();

        roles.AddRange(customerUsers.Select(id => new UserRoleEntity { UserId = id, RoleName = "Customer" }));
        roles.AddRange(staffUsers.Select(id => new UserRoleEntity { UserId = id, RoleName = "Staff" }));

        foreach (var id in remainingUsers.Take(5))
        {
            roles.Add(new UserRoleEntity { UserId = id, RoleName = "Manager" });
        }

        foreach (var id in remainingUsers.Skip(5).Take(5))
        {
            roles.Add(new UserRoleEntity { UserId = id, RoleName = "Admin" });
        }

        _context.UserRoles.AddRange(roles);
        await _context.SaveChangesAsync();
    }

    private async Task SeedCustomersAsync(SeedSettings settings, Random random)
    {
        var customerUsers = await _context.UserRoles
            .Where(r => r.RoleName == "Customer")
            .Select(r => r.UserId)
            .ToListAsync();

        var existingCustomerUserIds = await _context.Customers
            .IgnoreQueryFilters()
            .Select(c => c.UserId)
            .ToListAsync();

        var existingCustomerEmails = await _context.Customers
            .IgnoreQueryFilters()
            .Select(c => c.Email)
            .ToListAsync();

        var users = await _context.Users
            .Where(u => customerUsers.Contains(u.Id))
            .Where(u => !existingCustomerUserIds.Contains(u.Id))
            .Where(u => !existingCustomerEmails.Contains(u.Email))
            .ToListAsync();

        var customers = users
            .Take(settings.Customers)
            .Select(u => new Customer
            {
                UserId = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Phone = SeedData.RandomPhone(random),
                IsActive = true,
                IsVerified = true,
                LoyaltyPoints = random.Next(0, 300)
            })
            .ToList();

        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();
    }

    private async Task SeedStaffAsync(SeedSettings settings, Random random)
    {
        var staffUsers = await _context.UserRoles
            .Where(r => r.RoleName == "Staff" || r.RoleName == "Admin" || r.RoleName == "Manager")
            .Select(r => r.UserId)
            .Distinct()
            .ToListAsync();

        var users = await _context.Users
            .Where(u => staffUsers.Contains(u.Id))
            .Take(settings.Staff)
            .ToListAsync();

        var staff = users.Select(u => new Staff
        {
            UserId = u.Id,
            Phone = SeedData.RandomPhone(random),
            IsActive = true
        }).ToList();

        _context.Staff.AddRange(staff);
        await _context.SaveChangesAsync();
    }

    private async Task SeedMenuAsync(SeedSettings settings, string marker, Random random)
    {
        var categories = new List<Category>();
        for (var i = 0; i < settings.Categories; i++)
        {
            categories.Add(new Category
            {
                Name = $"{marker} Category {i + 1}",
                Description = "Seeded category",
                IsActive = true,
                DisplayOrder = i + 1
            });
        }

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync();

        var menuItems = new List<MenuItem>();
        for (var i = 0; i < settings.MenuItems; i++)
        {
            var category = categories[random.Next(categories.Count)];
            menuItems.Add(new MenuItem
            {
                CategoryId = category.Id,
                Name = $"{marker} Item {i + 1}",
                Description = "Seeded item",
                Price = Math.Round((decimal)(random.NextDouble() * 40 + 5), 2),
                IsAvailable = true,
                IsFeatured = random.Next(0, 10) == 0,
                PreparationTimeMinutes = random.Next(5, 30),
                Calories = random.Next(150, 900),
                SpiceLevel = random.Next(0, 6),
                MaxQuantityPerOrder = random.Next(1, 10)
            });
        }

        _context.MenuItems.AddRange(menuItems);
        await _context.SaveChangesAsync();

        var optionGroups = new List<MenuItemOptionGroup>();
        foreach (var item in menuItems)
        {
            for (var g = 0; g < settings.OptionGroupsPerItem; g++)
            {
                optionGroups.Add(new MenuItemOptionGroup
                {
                    ItemId = item.Id,
                    Name = $"{marker} Group {g + 1}",
                    SelectionType = OptionSelectionType.Single,
                    MinSelections = 0,
                    MaxSelections = 1,
                    DisplayOrder = g + 1
                });
            }
        }

        _context.MenuItemOptionGroups.AddRange(optionGroups);
        await _context.SaveChangesAsync();

        var options = new List<MenuItemOption>();
        foreach (var group in optionGroups)
        {
            for (var o = 0; o < settings.OptionsPerGroup; o++)
            {
                options.Add(new MenuItemOption
                {
                    OptionGroupId = group.Id,
                    Name = $"{marker} Option {o + 1}",
                    PriceAdjustment = Math.Round((decimal)(random.NextDouble() * 5), 2),
                    IsAvailable = true,
                    DisplayOrder = o + 1
                });
            }
        }

        _context.MenuItemOptions.AddRange(options);
        await _context.SaveChangesAsync();
    }

    private async Task SeedInventoryAsync(SeedSettings settings, string markerUpper, Random random)
    {
        var categories = new List<InventoryCategory>();
        for (var i = 0; i < settings.InventoryCategories; i++)
        {
            categories.Add(new InventoryCategory
            {
                Name = $"Inventory {i + 1}",
                Description = "Seeded inventory category"
            });
        }

        _context.InventoryCategories.AddRange(categories);
        await _context.SaveChangesAsync();

        var items = new List<InventoryItem>();
        for (var i = 0; i < settings.InventoryItems; i++)
        {
            var category = categories[random.Next(categories.Count)];
            var min = Math.Round((decimal)(random.NextDouble() * 20), 2);
            var current = Math.Round((decimal)(random.NextDouble() * 50), 2);
            items.Add(new InventoryItem
            {
                InventoryCategoryId = category.Id,
                Sku = $"{markerUpper}-SKU-{Guid.NewGuid():N}"[..Math.Min(45, $"{markerUpper}-SKU-".Length + 32)],
                Name = $"Ingredient {i + 1}",
                UnitOfMeasure = "kg",
                CurrentQuantity = current,
                MinimumQuantity = min,
                ReorderQuantity = Math.Round((decimal)(random.NextDouble() * 40), 2),
                UnitCost = Math.Round((decimal)(random.NextDouble() * 10 + 1), 2),
                SupplierName = "Seed Supplier",
                SupplierContact = "seed@supplier.local",
                IsActive = true
            });
        }

        _context.InventoryItems.AddRange(items);
        await _context.SaveChangesAsync();

        var menuItems = await _context.MenuItems.ToListAsync();
        var ingredientLinks = new List<MenuItemIngredient>();
        var pairSet = new HashSet<string>();
        foreach (var menu in menuItems)
        {
            var links = random.Next(1, 4);
            for (var i = 0; i < links; i++)
            {
                var inv = items[random.Next(items.Count)];
                var key = $"{menu.Id}-{inv.Id}";
                if (!pairSet.Add(key))
                {
                    continue;
                }

                ingredientLinks.Add(new MenuItemIngredient
                {
                    ItemId = menu.Id,
                    InventoryItemId = inv.Id,
                    QuantityRequired = Math.Round((decimal)(random.NextDouble() * 2 + 0.1), 3)
                });
            }
        }

        _context.MenuItemIngredients.AddRange(ingredientLinks);
        await _context.SaveChangesAsync();

        var staffIds = await _context.Staff.Select(s => s.Id).ToListAsync();
        var transactions = new List<InventoryTransaction>();
        for (var i = 0; i < settings.InventoryTransactions; i++)
        {
            var item = items[random.Next(items.Count)];
            var delta = Math.Round((decimal)(random.NextDouble() * 5), 2);
            if (random.Next(0, 2) == 0)
            {
                delta *= -1;
            }

            var newQty = Math.Max(0, item.CurrentQuantity + delta);
            item.CurrentQuantity = newQty;

            transactions.Add(new InventoryTransaction
            {
                InventoryItemId = item.Id,
                TransactionType = InventoryTransactionType.Adjustment,
                QuantityChange = delta,
                QuantityAfter = newQty,
                StaffId = staffIds.Count == 0 ? null : staffIds[random.Next(staffIds.Count)]
            });
        }

        _context.InventoryTransactions.AddRange(transactions);
        await _context.SaveChangesAsync();
    }

    private async Task SeedTablesAsync(SeedSettings settings, string markerUpper)
    {
        var tables = new List<RestaurantTable>();
        for (var i = 0; i < settings.Tables; i++)
        {
            tables.Add(new RestaurantTable
            {
                TableNumber = $"{markerUpper}-T-{i + 1}",
                Capacity = 2 + (i % 6),
                Status = TableStatus.Available
            });
        }

        _context.RestaurantTables.AddRange(tables);
        await _context.SaveChangesAsync();
    }

    private async Task SeedOrdersAsync(SeedSettings settings, string markerUpper, Random random)
    {
        var customers = await _context.Customers.ToListAsync();
        var menuItems = await _context.MenuItems.ToListAsync();
        var optionGroups = await _context.MenuItemOptionGroups.ToListAsync();
        var options = await _context.MenuItemOptions.ToListAsync();

        if (customers.Count == 0 || menuItems.Count == 0)
        {
            return;
        }

        var orders = new List<Order>();
        var orderCount = settings.Orders;

        for (var i = 0; i < orderCount; i++)
        {
            var customer = customers[random.Next(customers.Count)];
            var status = SeedData.RandomOrderStatus(random);
            var order = new Order
            {
                OrderNumber = $"{markerUpper}-ORD-{Guid.NewGuid():N}"[..20],
                CustomerId = customer.Id,
                OrderType = SeedData.RandomOrderType(random),
                OrderStatus = status,
                TaxRate = 0.08m
            };

            var itemsCount = random.Next(settings.OrderItemsMin, settings.OrderItemsMax + 1);
            decimal subtotal = 0m;

            for (var j = 0; j < itemsCount; j++)
            {
                var menu = menuItems[random.Next(menuItems.Count)];
                var groupsForItem = optionGroups.Where(g => g.ItemId == menu.Id).ToList();
                var selectedOptions = new List<MenuItemOption>();

                foreach (var group in groupsForItem)
                {
                    var groupOptions = options.Where(o => o.OptionGroupId == group.Id).ToList();
                    if (groupOptions.Count == 0) continue;
                    var pick = groupOptions[random.Next(groupOptions.Count)];
                    selectedOptions.Add(pick);
                }

                var quantity = random.Next(1, 4);
                var optionsTotal = selectedOptions.Sum(o => o.PriceAdjustment);
                var unitPrice = menu.Price + optionsTotal;
                var lineSubtotal = unitPrice * quantity;

                var orderItem = new OrderItem
                {
                    ItemId = menu.Id,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    Subtotal = lineSubtotal,
                    ItemStatus = OrderItemStatus.Pending
                };

                foreach (var opt in selectedOptions)
                {
                    orderItem.SelectedOptions.Add(new OrderItemOption
                    {
                        OptionId = opt.Id,
                        OptionGroupName = opt.OptionGroup.Name,
                        OptionName = opt.Name,
                        PriceAdjustment = opt.PriceAdjustment
                    });
                }

                order.OrderItems.Add(orderItem);
                subtotal += lineSubtotal;
            }

            order.Subtotal = subtotal;
            order.TaxAmount = Math.Round(subtotal * order.TaxRate, 2);
            order.TotalAmount = subtotal + order.TaxAmount + order.DeliveryFee + order.TipAmount - order.DiscountAmount;

            if (status == OrderStatus.Cancelled)
            {
                order.CancelledAt = DateTime.UtcNow;
                order.CancelledByType = CancelledByType.System;
            }

            orders.Add(order);
        }

        _context.Orders.AddRange(orders);
        await _context.SaveChangesAsync();
    }

    private async Task SeedPaymentsAsync(SeedSettings settings, Random random)
    {
        var orders = await _context.Orders
            .Where(o => o.OrderStatus == OrderStatus.Completed || o.OrderStatus == OrderStatus.Confirmed)
            .Take(settings.Payments)
            .ToListAsync();

        if (orders.Count == 0)
        {
            return;
        }

        var existingPaymentOrderIds = await _context.Payments
            .Select(p => p.OrderId)
            .ToListAsync();

        var payments = orders.Select(o => new Payment
        {
            OrderId = o.Id,
            PaymentMethod = PaymentMethod.Cash,
            Amount = o.TotalAmount,
            PaymentStatus = PaymentStatus.Completed,
            PaymentDate = DateTime.UtcNow
        })
            .Where(p => !existingPaymentOrderIds.Contains(p.OrderId))
            .ToList();

        if (payments.Count == 0)
        {
            return;
        }

        _context.Payments.AddRange(payments);
        await _context.SaveChangesAsync();
    }

    private async Task SeedReservationsAsync(SeedSettings settings, string markerUpper, Random random)
    {
        var customers = await _context.Customers.ToListAsync();
        var tables = await _context.RestaurantTables.ToListAsync();

        if (customers.Count == 0 || tables.Count == 0)
        {
            return;
        }

        var reservations = new List<Reservation>();
        var used = new HashSet<string>();

        for (var i = 0; i < settings.Reservations; i++)
        {
            var table = tables[random.Next(tables.Count)];
            var dayOffset = random.Next(0, 30);
            var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(dayOffset));
            var time = new TimeOnly(random.Next(11, 22), random.Next(0, 2) == 0 ? 0 : 30);

            var key = $"{table.Id}-{date}-{time}";
            if (!used.Add(key))
            {
                continue;
            }

            var customer = customers[random.Next(customers.Count)];
            reservations.Add(new Reservation
            {
                CustomerId = customer.Id,
                TableId = table.Id,
                ReservationDate = date,
                ReservationTime = time,
                PartySize = random.Next(1, 8),
                Status = ReservationStatus.Pending,
                ConfirmationCode = $"{markerUpper}-R-{Guid.NewGuid():N}"[..12]
            });
        }

        _context.Reservations.AddRange(reservations);
        await _context.SaveChangesAsync();
    }

    private async Task SeedReviewsAsync(SeedSettings settings, string marker, Random random)
    {
        var completedOrders = await _context.Orders
            .Where(o => o.OrderStatus == OrderStatus.Completed && o.CustomerId != null)
            .ToListAsync();

        if (completedOrders.Count == 0)
        {
            return;
        }

        var reviews = new List<Review>();
        foreach (var order in completedOrders.Take(settings.Reviews))
        {
            reviews.Add(new Review
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId!.Value,
                Rating = random.Next(3, 6),
                ReviewText = $"{marker} review for order {order.OrderNumber}",
                IsPublished = true
            });
        }

        _context.Reviews.AddRange(reviews);
        await _context.SaveChangesAsync();
    }

    private sealed class SeedSettings
    {
        public bool Enabled { get; init; } = true;
        public string Marker { get; init; } = "seed";
        public int Users { get; init; } = 200;
        public int Customers { get; init; } = 150;
        public int Staff { get; init; } = 30;
        public int Categories { get; init; } = 15;
        public int MenuItems { get; init; } = 120;
        public int OptionGroupsPerItem { get; init; } = 2;
        public int OptionsPerGroup { get; init; } = 4;
        public int Orders { get; init; } = 300;
        public int OrderItemsMin { get; init; } = 1;
        public int OrderItemsMax { get; init; } = 4;
        public int Payments { get; init; } = 220;
        public int Reservations { get; init; } = 120;
        public int Tables { get; init; } = 30;
        public int InventoryCategories { get; init; } = 10;
        public int InventoryItems { get; init; } = 200;
        public int InventoryTransactions { get; init; } = 400;
        public int Reviews { get; init; } = 120;

        public static SeedSettings FromConfiguration(IConfiguration configuration)
        {
            var section = configuration.GetSection("Seed");
            return new SeedSettings
            {
                Enabled = section.GetValue("Enabled", true),
                Marker = section.GetValue("Marker", "seed"),
                Users = section.GetValue("Counts:Users", 200),
                Customers = section.GetValue("Counts:Customers", 150),
                Staff = section.GetValue("Counts:Staff", 30),
                Categories = section.GetValue("Counts:Categories", 15),
                MenuItems = section.GetValue("Counts:MenuItems", 120),
                OptionGroupsPerItem = section.GetValue("Counts:OptionGroupsPerItem", 2),
                OptionsPerGroup = section.GetValue("Counts:OptionsPerGroup", 4),
                Orders = section.GetValue("Counts:Orders", 300),
                OrderItemsMin = section.GetValue("Counts:OrderItemsMin", 1),
                OrderItemsMax = section.GetValue("Counts:OrderItemsMax", 4),
                Payments = section.GetValue("Counts:Payments", 220),
                Reservations = section.GetValue("Counts:Reservations", 120),
                Tables = section.GetValue("Counts:Tables", 30),
                InventoryCategories = section.GetValue("Counts:InventoryCategories", 10),
                InventoryItems = section.GetValue("Counts:InventoryItems", 200),
                InventoryTransactions = section.GetValue("Counts:InventoryTransactions", 400),
                Reviews = section.GetValue("Counts:Reviews", 120)
            };
        }
    }

    private static class SeedData
    {
        public static readonly string[] Names =
        [
            "Alex","Sam","Lina","Omar","Sara","Noah","Mia","Ava","Liam","Emma","Ethan","Maya",
            "Zara","Ivy","Aria","Leo","Nora","Mason","Amir","Hana","Yara","Ola","Rami","Tia"
        ];

        public static string RandomPhone(Random random)
        {
            return $"+1-555-{random.Next(100, 999)}-{random.Next(1000, 9999)}";
        }

        public static OrderStatus RandomOrderStatus(Random random)
        {
            var values = new[] { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.Preparing, OrderStatus.Ready, OrderStatus.Completed, OrderStatus.Cancelled };
            return values[random.Next(values.Length)];
        }

        public static OrderType RandomOrderType(Random random)
        {
            var values = new[] { OrderType.Pickup, OrderType.Delivery, OrderType.DineIn };
            return values[random.Next(values.Length)];
        }
    }
}
