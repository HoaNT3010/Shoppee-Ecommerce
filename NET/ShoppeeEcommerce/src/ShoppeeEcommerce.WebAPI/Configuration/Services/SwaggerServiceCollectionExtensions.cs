using Microsoft.OpenApi;

namespace ShoppeeEcommerce.WebAPI.Configuration.Services
{
    public static class SwaggerServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerDocs(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Shoppee Ecommerce API",
                    Version = "v1",
                });
                c.EnableAnnotations();
            });
            return services;
        }
    }
}
