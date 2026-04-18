using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Infrastructure.Authentication;

namespace ShoppeeEcommerce.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services)
        {
            services.AddAuthenticationServices();
            return services;
        }
    }
}
