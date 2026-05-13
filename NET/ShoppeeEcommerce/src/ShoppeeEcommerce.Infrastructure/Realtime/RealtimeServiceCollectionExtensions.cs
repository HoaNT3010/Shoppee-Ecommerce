using Microsoft.Extensions.DependencyInjection;
using ShoppeeEcommerce.Application.Abstractions.Realtime;

namespace ShoppeeEcommerce.Infrastructure.Realtime
{
    public static class RealtimeServiceCollectionExtensions
    {
        public static IServiceCollection AddRealtimeServices(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddScoped<IOrderRealtimeNotifier, OrderRealtimeNotifier>();

            return services;
        }
    }
}
