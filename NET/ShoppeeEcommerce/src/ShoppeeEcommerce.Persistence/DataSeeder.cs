using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Domain.Entities.Identity;

namespace ShoppeeEcommerce.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // Roles
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new Role() { Name = roleName });
                }
            }

            // Admins
            await CreateUserIfNotExists(userManager, "admin1@gmail.com", "AdminPass123!", "Admin", "John", "Administrator");
            await CreateUserIfNotExists(userManager, "admin2@gmail.com", "AdminPass123!", "Admin");

            // Customers
            await CreateUserIfNotExists(userManager, "customer1@gmail.com", "CusPass123!", "Customer", "Alice", "Customer");
            await CreateUserIfNotExists(userManager, "customer2@gmail.com", "CustPass123!", "Customer");
        }

        private static async Task CreateUserIfNotExists(UserManager<User> userManager,
            string email,
            string password,
            string role,
            string? firstName = null,
            string? lastName = null)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
