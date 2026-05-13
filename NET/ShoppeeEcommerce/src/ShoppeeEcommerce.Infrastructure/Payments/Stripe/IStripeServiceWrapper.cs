using Stripe;

namespace ShoppeeEcommerce.Infrastructure.Payments.Stripe
{
    internal interface IStripeServiceWrapper
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(PaymentIntentCreateOptions options, CancellationToken ct);
        Task<Refund> CreateRefundAsync(RefundCreateOptions options, CancellationToken ct);
        Event ConstructEvent(string json, string signature, string webhookSecret);
    }
}
