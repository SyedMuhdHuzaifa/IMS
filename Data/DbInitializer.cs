using IMS.Data;
using IMS.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

            try
            {
                // Ensure pending migrations are applied
                if (context.Database.IsRelational())
                {
                    await context.Database.MigrateAsync();
                }

                // 1. Seed Roles
                string[] roleNames = { "Admin", "Manager", "WarehouseStaff" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var role = new ApplicationRole
                        {
                            Name = roleName,
                            Description = $"{roleName} role with designated permissions"
                        };
                        await roleManager.CreateAsync(role);
                        logger.LogInformation("Seeded role: {Role}", roleName);
                    }
                }

                // 2. Seed Default Users
                var usersToSeed = new[]
                {
                    new { UserName = "admin", Email = "admin@ims.com", FullName = "System Administrator", Role = "Admin", Password = "AdminPassword123!" },
                    new { UserName = "manager", Email = "manager@ims.com", FullName = "Inventory Operations Manager", Role = "Manager", Password = "ManagerPassword123!" },
                    new { UserName = "staff", Email = "staff@ims.com", FullName = "Warehouse Associate", Role = "WarehouseStaff", Password = "StaffPassword123!" }
                };

                foreach (var u in usersToSeed)
                {
                    var existingUser = await userManager.FindByEmailAsync(u.Email);
                    if (existingUser == null)
                    {
                        var newUser = new ApplicationUser
                        {
                            UserName = u.UserName,
                            Email = u.Email,
                            FullName = u.FullName,
                            EmailConfirmed = true,
                            IsActive = true
                        };

                        var createResult = await userManager.CreateAsync(newUser, u.Password);
                        if (createResult.Succeeded)
                        {
                            await userManager.AddToRoleAsync(newUser, u.Role);
                            logger.LogInformation("Seeded user: {UserName} ({Role})", u.UserName, u.Role);
                        }
                    }
                }

                // 3. Seed Warehouses
                if (!await context.Warehouses.AnyAsync())
                {
                    var warehouses = new[]
                    {
                        new Warehouse
                        {
                            Name = "Main Central Warehouse",
                            Code = "WH-MAIN",
                            Location = "Central Distribution Center, Building A",
                            Description = "Primary inventory storage and fulfillment warehouse",
                            IsActive = true
                        },
                        new Warehouse
                        {
                            Name = "East Regional Facility",
                            Code = "WH-EAST",
                            Location = "Metro Logistics Hub, Bay 4",
                            Description = "Secondary distribution and transfer center",
                            IsActive = true
                        }
                    };

                    await context.Warehouses.AddRangeAsync(warehouses);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded warehouses: WH-MAIN, WH-EAST");
                }

                // 4. Seed Categories
                if (!await context.Categories.AnyAsync())
                {
                    var categories = new[]
                    {
                        new Category { Name = "Electronics", Description = "Computer hardware, peripherals, and electronic equipment" },
                        new Category { Name = "Office Supplies", Description = "Desk accessories, paper products, and stationery" },
                        new Category { Name = "Raw Materials", Description = "Production materials, cables, and packaging supplies" }
                    };

                    await context.Categories.AddRangeAsync(categories);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded initial categories.");
                }

                // 5. Seed Suppliers
                if (!await context.Suppliers.AnyAsync())
                {
                    var suppliers = new[]
                    {
                        new Supplier { Name = "TechDistro Global", Email = "orders@techdistro.com", Phone = "+1-555-0199", Address = "100 Technology Blvd, Suite 400" },
                        new Supplier { Name = "Global Office Supplies Ltd", Email = "sales@globaloffice.com", Phone = "+1-555-0144", Address = "45 Enterprise Way, Dock 2" }
                    };

                    await context.Suppliers.AddRangeAsync(suppliers);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded initial suppliers.");
                }

                // 6. Seed Customers
                if (!await context.Customers.AnyAsync())
                {
                    var customers = new[]
                    {
                        new Customer { Name = "Apex Industrial Corp", Email = "procurement@apexcorp.com", Phone = "+1-555-0211", Address = "750 Commercial Ave, Floor 12" },
                        new Customer { Name = "Nexus Retailers Inc", Email = "orders@nexusretail.com", Phone = "+1-555-0288", Address = "1200 Market Street, Bay Area" }
                    };

                    await context.Customers.AddRangeAsync(customers);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded initial customers.");
                }

                // 7. Seed Products
                if (!await context.Products.AnyAsync())
                {
                    var electronics = await context.Categories.FirstAsync(c => c.Name == "Electronics");
                    var office = await context.Categories.FirstAsync(c => c.Name == "Office Supplies");

                    var products = new[]
                    {
                        new Product
                        {
                            Name = "Dell Latitude 5540 15.6\"",
                            SKU = "PROD-LAPTOP-01",
                            Description = "Enterprise core business laptop with Intel Core i7, 16GB RAM, 512GB SSD",
                            PurchasePrice = 900.00m,
                            SellingPrice = 1200.00m,
                            ReorderLevel = 5,
                            CategoryId = electronics.Id,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "Logitech MX Master 3S Wireless Mouse",
                            SKU = "PROD-MOUSE-01",
                            Description = "Ergonomic wireless productivity mouse with ultra-fast scroll wheel",
                            PurchasePrice = 60.00m,
                            SellingPrice = 99.99m,
                            ReorderLevel = 10,
                            CategoryId = electronics.Id,
                            IsActive = true
                        },
                        new Product
                        {
                            Name = "A4 Multipurpose Copy Paper Box (5 Reams)",
                            SKU = "PROD-PAPER-01",
                            Description = "High-quality 80gsm white printing paper box (2500 sheets)",
                            PurchasePrice = 20.00m,
                            SellingPrice = 35.00m,
                            ReorderLevel = 25,
                            CategoryId = office.Id,
                            IsActive = true
                        }
                    };

                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded initial products.");
                }

                // 8. Seed Initial Warehouse Stock
                if (!await context.WarehouseStocks.AnyAsync())
                {
                    var whMain = await context.Warehouses.FirstAsync(w => w.Code == "WH-MAIN");
                    var whEast = await context.Warehouses.FirstOrDefaultAsync(w => w.Code == "WH-EAST");
                    var products = await context.Products.ToListAsync();

                    var stocks = new List<WarehouseStock>();
                    foreach (var product in products)
                    {
                        int mainQty = product.SKU switch
                        {
                            "PROD-LAPTOP-01" => 50,
                            "PROD-MOUSE-01" => 120,
                            "PROD-PAPER-01" => 250,
                            _ => 30
                        };

                        stocks.Add(new WarehouseStock
                        {
                            WarehouseId = whMain.Id,
                            ProductId = product.Id,
                            QuantityOnHand = mainQty
                        });

                        if (whEast != null)
                        {
                            int eastQty = product.SKU switch
                            {
                                "PROD-LAPTOP-01" => 10,
                                "PROD-MOUSE-01" => 35,
                                "PROD-PAPER-01" => 60,
                                _ => 10
                            };

                            stocks.Add(new WarehouseStock
                            {
                                WarehouseId = whEast.Id,
                                ProductId = product.Id,
                                QuantityOnHand = eastQty
                            });
                        }
                    }

                    await context.WarehouseStocks.AddRangeAsync(stocks);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded initial warehouse inventory levels.");
                }

                logger.LogInformation("Database initialization & seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
