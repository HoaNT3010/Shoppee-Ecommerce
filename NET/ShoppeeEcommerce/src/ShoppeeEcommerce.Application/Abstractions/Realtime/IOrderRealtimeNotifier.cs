namespace ShoppeeEcommerce.Application.Abstractions.Realtime
{
    public interface IOrderRealtimeNotifier
    {
        Task NotifyOrderUpdated(Guid orderId, string status, CancellationToken cancellationToken = default!);
    }
}
