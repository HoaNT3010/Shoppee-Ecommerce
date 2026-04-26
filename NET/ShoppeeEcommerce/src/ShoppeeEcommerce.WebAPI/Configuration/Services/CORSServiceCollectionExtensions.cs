namespace ShoppeeEcommerce.WebAPI.Configuration.Services
{
    public static class CORSServiceCollectionExtensions
    {
        internal const string AllowedAllOriginsPolicy = "AllowAllOrigins";
        internal const string AdminOriginPolicy = "AllowAdminOrigin";
        internal const string CustomerOriginPolicy = "AllowCustomerOrigin";

        public static IServiceCollection ConfigureCORSPolicies(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                // Allow all for development
                options.AddPolicy(AllowedAllOriginsPolicy, policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .SetIsOriginAllowed(_ => true);
                });
                options.AddPolicy(AdminOriginPolicy, policy =>
                {
                    // Default Vite port
                    policy.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
                options.AddPolicy(CustomerOriginPolicy, policy =>
                {
                    // Customer site's url can change latter
                    policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            return services;
        }
    }
}
