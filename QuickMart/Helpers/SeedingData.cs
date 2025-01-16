using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace QuickMart.Helpers
{
    public static class SeedingData
    {
        public static async Task SeedData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                try
                {
                    var QuickMartDbContext = scope.ServiceProvider.GetRequiredService<QuickMartContext>();

                    await QuickMartDbContext.Database.MigrateAsync();

                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    var adminEmail = "admin@gmail.com";
                    var adminPassword = "Admin@123";
                    string[] roles = { "Admin", "Customer" };

                    // Seed roles
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    // Seed admin user
                    if (await userManager.FindByEmailAsync(adminEmail) == null)
                    {
                        var adminUser = new ApplicationUser
                        {
                            UserName = adminEmail,
                            Email = adminEmail,
                            FirstName="Admin",
                            LastName ="Admin",
                            Address = "Admin Address",
                            FullName= "Admin Admin",
                            EmailConfirmed = true
                        };

                        if ((await userManager.CreateAsync(adminUser, adminPassword)).Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, "Admin");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during initialization: {ex.Message}");
                }
            }
        }
    }
}
