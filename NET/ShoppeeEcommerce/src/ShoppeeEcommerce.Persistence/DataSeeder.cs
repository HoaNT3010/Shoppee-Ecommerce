using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Domain.Constants;
using ShoppeeEcommerce.Domain.Entities.Core;
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
            string[] roleNames = { ApplicationRole.Admin, ApplicationRole.Customer };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new Role() { Name = roleName });
                }
            }

            // Admins
            await CreateUserIfNotExists(userManager, "admin1", "admin1@gmail.com", "AdminPass123!", "Admin", "John", "Administrator");
            await CreateUserIfNotExists(userManager, "admin2", "admin2@gmail.com", "AdminPass123!", "Admin");

            // Customers
            await CreateUserIfNotExists(userManager, "customer1", "customer1@gmail.com", "CusPass123!", "Customer", "Alice", "Customer");
            await CreateUserIfNotExists(userManager, "customer2", "customer2@gmail.com", "CustPass123!", "Customer");

            // Categories
            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            await CreateCategoriesIfNotExists(dbContext, userManager);
        }

        private static async Task CreateUserIfNotExists(UserManager<User> userManager,
            string userName,
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
                    UserName = userName,
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

        private static async Task CreateCategoriesIfNotExists(AppDbContext dbContext, UserManager<User> userManager)
        {
            if (await dbContext.Categories.AnyAsync()) return;

            var admin = await userManager.FindByNameAsync("admin1");
            Guid? creatorId = admin?.Id;

            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Electronics", Description = "Electronic gadgets and devices", CreatorId = creatorId },
                new Category { Id = Guid.NewGuid(), Name = "Books", Description = "Books across genres", CreatorId = creatorId },
                new Category { Id = Guid.NewGuid(), Name = "Clothing", Description = "Apparel for men and women", CreatorId = creatorId },
                new Category { Id = Guid.NewGuid(), Name = "Home & Kitchen", Description = "Home and kitchen appliances", CreatorId = creatorId },
                new Category { Id = Guid.NewGuid(), Name = "Beauty & Personal Care", Description = "Cosmetics and personal care items", CreatorId = creatorId }
            };

            dbContext.Categories.AddRange(categories);
            await dbContext.SaveChangesAsync();
        }
    }
}
