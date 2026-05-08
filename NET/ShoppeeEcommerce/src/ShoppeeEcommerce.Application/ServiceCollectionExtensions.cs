using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.Implementations.Services;
using System.Reflection;

namespace ShoppeeEcommerce.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddScoped<ICartService, CartService>();

            return services;
        }
    }
}
