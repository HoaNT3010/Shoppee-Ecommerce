using ShoppeeEcommerce.Domain.Constants;

namespace ShoppeeEcommerce.WebAPI.Configuration.Services
{
    public static class AuthorizationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthorizationServices(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Admin
                options.AddPolicy(AuthorizationPolicies.Admin, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(ApplicationRole.Admin);
                });
                // Customer
                options.AddPolicy(AuthorizationPolicies.Customer, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(ApplicationRole.Customer);
                });
                // Authenticated user
                options.AddPolicy(AuthorizationPolicies.AuthenticatedUser, policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
            });
            return services;
        }
    }

    public static class AuthorizationPolicies
    {
        public const string Admin = nameof(Admin);
        public const string Customer = nameof(Customer);
        public const string AuthenticatedUser = nameof(AuthenticatedUser);
    }
}
