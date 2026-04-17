using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShoppeeEcommerce.Persistence
{
    public static class ServiceCollectionExtensions
    {
        const string ConnectionStringName = "Database";

        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                // For development
                options.EnableSensitiveDataLogging(true);
                options.UseSqlServer(configuration.GetConnectionString(ConnectionStringName));
            });
            return services;
        }
    }
}
