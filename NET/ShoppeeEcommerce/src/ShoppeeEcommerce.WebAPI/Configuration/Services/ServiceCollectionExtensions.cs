using FluentValidation;
using ShoppeeEcommerce.WebAPI.Middlewares;
using ShoppeeEcommerce.WebAPI.Utilities;
using System.Reflection;

namespace ShoppeeEcommerce.WebAPI.Configuration.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebAPIServices(
            this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalValidationFilter>();
            });
            services.AddOpenApi();
            services.AddHealthChecks();
            services.AddApplicationIdentity();
            services.AddAuthenticationServices();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
