using ErrorOr;

namespace ShoppeeEcommerce.Domain.Errors
{
    public static partial class Errors
    {
        public static class PaymentErrors
        {
            public static Error Stripe_InvalidWebhookSignature() =>
                Error.Validation(
                    "Payment.Stripe.InvalidWebhookSignature",
                    "Webhook request contains invalid Stripe signature.");
            public static Error Stripe_PaymentNotFoundWithIntentId(string intentId) =>
                Error.Validation(
                    "Payment.Stripe.PaymentNotFoundWithIntentId",
                    $"No payment found with Stripe intent ID '{intentId}'");

            public static Error Stripe_MarkPaymentAsSucceededFailed() =>
                Error.Failure(
                    "Payment.Stripe.MarkPaymentAsSucceededFailed",
                    "Unexpected error occurred when trying to mark Stripe payment as succeeded.");
            public static Error Stripe_MarkPaymentAsCancelledFailed() =>
                Error.Failure(
                    "Payment.Stripe.MarkPaymentAsCancelledFailed",
                    "Unexpected error occurred when trying to mark Stripe payment as cancelled.");
            public static Error Stripe_MarkPaymentAsRefundedFailed() =>
                Error.Failure(
                    "Payment.Stripe.MarkPaymentAsRefundedFailed",
                    "Unexpected error occurred when trying to mark Stripe payment as refunded.");
            public static Error Stripe_MarkPaymentAsFailedUnsuccessful() =>
                Error.Failure(
                    "Payment.Stripe.MarkPaymentAsFailedUnsuccessful",
                    "Unexpected error occurred when trying to mark Stripe payment as failed.");
            public static Error Stripe_CreatePaymentIntentFailed() =>
                Error.Failure(
                    "Payment.Stripe.CreatePaymentIntentFailed",
                    "Unexpected error occurred when trying to create Stripe payment intent for order.");
            public static Error Stripe_EventNotSupported() =>
                Error.Validation(
                    "Payment.Stripe.EventNotSupported",
                    "Stripe event not supported.");

            public static Error Order_NotPayable() =>
                Error.Validation(
                    "Payment.Order.NotPayable",
                    "Order is not payable. Cannot create payment for order.");
            public static Error Order_HasExistingPayment() =>
                Error.Conflict(
                    "Payment.Order.HasExistingPayment",
                    "Order has existing payment, please process the prior payment first.");

            public static Error NotFoundWithId(string id) =>
                Error.NotFound(
                    "Payment.NotFoundWithId",
                    $"Payment with ID '{id}' was not found.");
            public static Error NotRefundable(string paymentStatus) =>
                Error.Validation(
                    "Payment.NotRefundable",
                    $"Only successful payment is refundable. Current payment's status is '{paymentStatus}'.");
            public static Error Stripe_RefundedPaymentFailed() =>
                Error.Failure(
                    "Payment.Stripe.RefundedPaymentFailed",
                    "Unexpected error occurred when trying to refund Stripe payment and purchase order.");
        }
    }
}
