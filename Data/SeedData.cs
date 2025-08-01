using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductManagementApp.Data
{
    public static class SeedData
    {
        private const string DefaultProfilePicture = "/images/default-avatar.png";

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = services.GetRequiredService<ILogger<Program>>();

            try
            {
                // Create roles if they don't exist
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    logger.LogInformation("Created Admin role");
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                    logger.LogInformation("Created User role");
                }

                var adminEmail = "admin@gmail.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = "admin111",
                        Email = adminEmail,
                        FullName = "Administrator",
                        EmailConfirmed = true,
                        ProfilePicture = DefaultProfilePicture
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.LogInformation("Created default admin user");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors));
                    }
                }
                var userEmail = "gabriellak@gmail.com";
                var regularUser = await userManager.FindByEmailAsync(userEmail);
                if (regularUser == null)
                {
                    regularUser = new ApplicationUser
                    {
                        UserName = "gabrielaell11",
                        Email = userEmail,
                        FullName = "Gabriella",
                        EmailConfirmed = true,
                        ProfilePicture = DefaultProfilePicture
                    };

                    var result = await userManager.CreateAsync(regularUser, "Gabriella@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(regularUser, "User");
                        logger.LogInformation("Created default regular user");
                    }
                    else
                    {
                        logger.LogError("Failed to create regular user: {Errors}", string.Join(", ", result.Errors));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
}