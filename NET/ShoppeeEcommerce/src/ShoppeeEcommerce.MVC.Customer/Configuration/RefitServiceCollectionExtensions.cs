using Refit;
using ShoppeeEcommerce.MVC.Customer.API;

namespace ShoppeeEcommerce.MVC.Customer.Configuration
{
    public static class RefitServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureRefit(this IServiceCollection services,
            IConfiguration configuration)
        {
            string apiUrl = configuration["ApiUrl"] ?? "http://localhost:8080/api/v1";

            services.AddRefitClient<IAuthApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl));
            // Add handler to handle auto refresh/rotate token
            services.AddTransient<ApiAuthHandler>();
            // Only register handler in non-auth api
            services
                .AddRefitClient<ICategoriesApi>()
                .AddRefitClient<IProductsApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl))
                .AddHttpMessageHandler<ApiAuthHandler>();

            services.AddTransient<CartOwnerHeaderHandler>();
            services
                .AddRefitClient<ICartApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl))
                .AddHttpMessageHandler<ApiAuthHandler>()
                .AddHttpMessageHandler<CartOwnerHeaderHandler>();

            return services;
        }
    }
}
