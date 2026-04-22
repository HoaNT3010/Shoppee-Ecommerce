using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Persistence.Repositories;

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
            services.AddRepositories();
            services.AddUnitOfWork();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(EFRepository<,>));

            return services;
        }

        private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, EFUnitOfWork>();

            return services;
        }
    }
}
