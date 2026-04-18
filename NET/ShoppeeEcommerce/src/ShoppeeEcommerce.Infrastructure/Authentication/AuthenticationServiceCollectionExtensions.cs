using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Application.Abstractions.Authentication;

namespace ShoppeeEcommerce.Infrastructure.Authentication
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services)
        {
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
            return services;
        }
    }
}
