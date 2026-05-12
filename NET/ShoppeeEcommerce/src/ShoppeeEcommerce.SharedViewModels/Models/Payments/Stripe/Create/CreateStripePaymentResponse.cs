namespace ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Create
{
    public class CreateStripePaymentResponse
    {
        public string StripePaymentIntentId { get; set; } = string.Empty;
        public string StripeClientSecret { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
    }
}
