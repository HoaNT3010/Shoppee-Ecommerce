using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Infrastructure.Authentication;
using ShoppeeEcommerce.Infrastructure.Storage.CloudinaryFS;

namespace ShoppeeEcommerce.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthenticationServices();
            services.AddCloudinary(configuration);
            return services;
        }
    }
}
