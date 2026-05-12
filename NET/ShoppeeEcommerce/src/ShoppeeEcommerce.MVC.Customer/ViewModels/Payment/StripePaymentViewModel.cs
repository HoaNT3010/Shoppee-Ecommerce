using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Create;

namespace ShoppeeEcommerce.MVC.Customer.ViewModels.Payment
{
    public class StripePaymentViewModel
    {
        public CreateStripePaymentResponse Payment { get; set; }
        public string PublishableKey { get; set; } = string.Empty;
    }
}
