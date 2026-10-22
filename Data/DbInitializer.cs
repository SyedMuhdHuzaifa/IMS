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

                // 2. Seed Default Admin User
                const string adminEmail = "admin@ims.com";
                const string adminUserName = "admin";
                const string adminPassword = "AdminPassword123!";

                var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
                if (existingAdmin == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = adminUserName,
                        Email = adminEmail,
                        FullName = "System Administrator",
                        EmailConfirmed = true,
                        IsActive = true
                    };

                    var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.LogInformation("Seeded admin user: {Email} (Password: {Password})", adminEmail, adminPassword);
                    }
                    else
                    {
                        logger.LogWarning("Failed to seed admin user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    }
                }

                // 3. Seed Primary Warehouse
                if (!await context.Warehouses.AnyAsync())
                {
                    var defaultWarehouse = new Warehouse
                    {
                        Name = "Main Central Warehouse",
                        Code = "WH-MAIN",
                        Location = "Central Distribution Center, Building A",
                        Description = "Primary inventory storage and fulfillment warehouse",
                        IsActive = true
                    };

                    await context.Warehouses.AddAsync(defaultWarehouse);
                    logger.LogInformation("Seeded primary warehouse: {Code}", defaultWarehouse.Code);
                }

                // 4. Seed Standard Categories
                if (!await context.Categories.AnyAsync())
                {
                    var categories = new[]
                    {
                        new Category { Name = "Electronics", Description = "Computer hardware, peripherals, and electronic equipment" },
                        new Category { Name = "Office Supplies", Description = "Desk accessories, paper products, and stationery" },
                        new Category { Name = "Raw Materials", Description = "Production materials, cables, and packaging supplies" }
                    };

                    await context.Categories.AddRangeAsync(categories);
                    logger.LogInformation("Seeded initial categories.");
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Database initialization & seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
