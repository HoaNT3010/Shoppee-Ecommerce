using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Application.Abstractions.Payments.Stripe;
using Stripe;

namespace ShoppeeEcommerce.Infrastructure.Payments.Stripe
{
    public static class StripeServiceCollectionExtensions
    {
        public static IServiceCollection AddStripe(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IStripeServiceWrapper, StripeServiceWrapper>();
            services.AddScoped<IStripePaymentService, StripePaymentService>();
            var stripeSettings = configuration
                .GetSection(StripeOptions.SectionName)
                .Get<StripeOptions>()!;
            StripeConfiguration.ApiKey = stripeSettings.SecretKey;

            return services;
        }
    }
}
