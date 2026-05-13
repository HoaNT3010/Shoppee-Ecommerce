using Stripe;

namespace ShoppeeEcommerce.Infrastructure.Payments.Stripe
{
    internal class StripeServiceWrapper
        : IStripeServiceWrapper
    {
        public Event ConstructEvent(string json, string signature, string webhookSecret)
        {
            return EventUtility.ConstructEvent(json, signature, webhookSecret, throwOnApiVersionMismatch: false);
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(PaymentIntentCreateOptions options, CancellationToken ct)
        {
            return await new PaymentIntentService().CreateAsync(options, cancellationToken: ct);
        }

        public async Task<Refund> CreateRefundAsync(RefundCreateOptions options, CancellationToken ct)
        {
            return await new RefundService().CreateAsync(options, cancellationToken: ct);
        }
    }
}
