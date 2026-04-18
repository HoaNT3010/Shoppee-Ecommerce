using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ShoppeeEcommerce.WebAPI.Configuration.Services
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationServices(
            this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer();
            services.ConfigureOptions<JwtBearerOptionsSetup>();

            return services;
        }
    }
}
