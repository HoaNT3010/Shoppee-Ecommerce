using ShoppeeEcommerce.Infrastructure.Authentication;

namespace ShoppeeEcommerce.WebAPI.Configuration
{
    internal static class OptionsConfiguration
    {
        internal static IServiceCollection ConfigureServicesOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();
            return services;
        }
    }
}
