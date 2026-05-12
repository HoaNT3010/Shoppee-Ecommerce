using ErrorOr;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.Abstractions.Payments.Stripe
{
    public interface IStripePaymentService
    {
        Task<ErrorOr<Payment>> CreatePaymentIntentAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
        Task<ErrorOr<Payment>> HandleWebhookAsync(string json, string stripeSignature, CancellationToken cancellationToken = default);
        Task<ErrorOr<Payment>> RefundAsync(Guid paymentId, decimal? amount = null, CancellationToken cancellationToken = default);
    }
}
