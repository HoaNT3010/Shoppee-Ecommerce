using ShoppeeEcommerce.Domain.Common;
using ShoppeeEcommerce.Domain.Entities.Base;
using ShoppeeEcommerce.Domain.Entities.Identity;
using ShoppeeEcommerce.Domain.Enums;

namespace ShoppeeEcommerce.Domain.Entities.Core
{
    public sealed class Payment : TrackableEntity<Guid>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Order Order { get; set; } = default!;
        public User User { get; set; } = default!;

        public string StripePaymentIntentId { get; set; } = string.Empty;
        public string? StripeClientSecret { get; set; }
        public string? StripeChargeId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; } = PaymentMethod.Card;

        public DateTime? PaidDate { get; set; }
        public DateTime? RefundedDate { get; set; }
        public DateTime? FailedDate { get; set; }

        public string? FailureReason { get; set; }
        public decimal? RefundedAmount { get; set; }

        public static Payment Create(
            Guid orderId,
            Guid userId,
            decimal amount,
            string stripePaymentIntentId,
            string stripeClientSecret,
            string currency = "USD")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stripePaymentIntentId);
            ArgumentException.ThrowIfNullOrWhiteSpace(stripeClientSecret);

            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));
            return new Payment
            {
                OrderId = orderId,
                UserId = userId,
                Amount = amount,
                Currency = currency.ToUpperInvariant(),
                StripePaymentIntentId = stripePaymentIntentId,
                StripeClientSecret = stripeClientSecret,
                Status = PaymentStatus.Pending,
            };
        }

        public void MarkAsSucceeded(string chargeId, DateTime? paidDate = null)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException($"Cannot mark payment as succeeded from status '{Status}'.");
            ArgumentException.ThrowIfNullOrWhiteSpace(chargeId);

            StripeChargeId = chargeId;
            Status = PaymentStatus.Succeeded;
            PaidDate = paidDate ?? DateTime.UtcNow;
            StripeClientSecret = null;
        }

        public void MarkAsFailed(string? reason, DateTime? failedDate = null)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException($"Cannot mark payment as failed from status '{Status}'.");

            Status = PaymentStatus.Failed;
            FailureReason = reason;
            FailedDate = failedDate ?? DateTime.UtcNow;
        }

        public void MarkAsRefunded(decimal? refundAmount = null, DateTime? refundDate = null)
        {
            if (Status != PaymentStatus.Succeeded)
                throw new InvalidOperationException("Only succeeded payments can be refunded.");

            var amount = refundAmount ?? Amount;

            if (amount <= 0 || amount > Amount)
                throw new ArgumentException("Refund amount must be between 0 and the original payment amount.");

            RefundedAmount = amount;
            RefundedDate = refundDate ?? DateTime.UtcNow;
            Status = amount < Amount
                ? PaymentStatus.PartiallyRefunded
                : PaymentStatus.Refunded;
        }

        public void MarkAsCancelled(DateTime? cancellDate = null)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be cancelled.");

            Status = PaymentStatus.Cancelled;
            this.SetUpdatedDateTime(cancellDate);
        }
    }
}
