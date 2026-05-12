using ShoppeeEcommerce.Infrastructure.Authentication;
using ShoppeeEcommerce.Infrastructure.Payments.Stripe;

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
            services.AddOptions<StripeOptions>()
                .BindConfiguration(StripeOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
