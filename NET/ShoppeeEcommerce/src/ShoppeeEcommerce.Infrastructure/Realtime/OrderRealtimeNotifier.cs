using Microsoft.AspNetCore.SignalR;
using ShoppeeEcommerce.Application.Abstractions.Realtime;
using ShoppeeEcommerce.Infrastructure.Realtime.Hubs;

namespace ShoppeeEcommerce.Infrastructure.Realtime
{
    internal class OrderRealtimeNotifier(
        IHubContext<OrderHub> hub)
        : IOrderRealtimeNotifier
    {
        public Task NotifyOrderUpdated(
            Guid orderId,
            string status,
            CancellationToken cancellationToken = default)
        {
            return hub.Clients
                .Group($"order-{orderId}")
                .SendAsync("OrderUpdated", new
                {
                    orderId,
                    status
                }, cancellationToken);
        }
    }
}
