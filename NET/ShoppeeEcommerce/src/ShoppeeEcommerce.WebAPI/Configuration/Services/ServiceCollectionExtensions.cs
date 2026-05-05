using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Login;
using ShoppeeEcommerce.WebAPI.Middlewares;
using ShoppeeEcommerce.WebAPI.Utilities;
using System.Text.Json.Serialization;

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
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddOpenApi();
            services.AddHealthChecks();
            services.AddApplicationIdentity();
            services.AddAuthenticationServices();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            services.AddValidatorsFromAssembly(typeof(LoginRequestValidator).Assembly);
            services.AddSwaggerDocs();
            services.AddAuthorizationServices();
            services.AddApiVersioningServices();
            services.ConfigureCORSPolicies();

            return services;
        }
    }
}
