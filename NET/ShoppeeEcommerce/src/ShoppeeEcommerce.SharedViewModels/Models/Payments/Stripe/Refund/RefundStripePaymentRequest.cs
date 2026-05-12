using Microsoft.AspNetCore.Mvc;

namespace ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Refund
{
    public class RefundStripePaymentRequest
    {
        [FromRoute(Name = "orderId")]
        public string OrderId { get; set; } = string.Empty;

        [FromRoute(Name = "paymentId")]
        public string PaymentId { get; set; } = string.Empty;

        [FromBody]
        public RefundAmountRequest Body { get; set; } = new();
    }

    public class RefundAmountRequest
    {
        public decimal Amount { get; set; }
    }
}
