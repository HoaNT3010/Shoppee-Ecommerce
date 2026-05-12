using FluentValidation;
using ShoppeeEcommerce.SharedViewModels.Validators;

namespace ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Refund
{
    internal class RefundStripePaymentRequestValidator
        : AbstractValidator<RefundStripePaymentRequest>
    {
        public RefundStripePaymentRequestValidator()
        {
            RuleFor(x => x.PaymentId).MustBeGuid();
            RuleFor(x => x.OrderId).MustBeGuid();
            RuleFor(x => x.Body.Amount)
                .GreaterThan(0)
                .WithMessage("Refund amount must be greater than 0.");
        }
    }
}
