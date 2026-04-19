using ShoppeeEcommerce.WebAPI.Middlewares;

namespace ShoppeeEcommerce.WebAPI.Configuration.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebAPIServices(
            this IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddHealthChecks();
            services.AddApplicationIdentity();
            services.AddAuthenticationServices();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            return services;
        }
    }
}
