using ErrorOr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Payments.Stripe;
using ShoppeeEcommerce.Application.Abstractions.Realtime;
using ShoppeeEcommerce.Application.UseCases.Orders.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Payments.Common.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.Domain.Errors;
using Stripe;

namespace ShoppeeEcommerce.Infrastructure.Payments.Stripe
{
    internal class StripePaymentService : IStripePaymentService
    {
        readonly IRepository<Payment, Guid> _paymentRepo;
        readonly IRepository<Order, Guid> _orderRepo;
        readonly IUnitOfWork _uow;
        readonly StripeOptions _options;
        readonly ILogger<StripePaymentService> _logger;
        readonly IOrderRealtimeNotifier _orderNotifier;

        public StripePaymentService(
            IRepository<Payment, Guid> paymentRepo,
            IRepository<Order, Guid> orderRepo,
            IUnitOfWork uow,
            IOptions<StripeOptions> options,
            ILogger<StripePaymentService> logger,
            IOrderRealtimeNotifier orderNotifier)
        {
            _paymentRepo = paymentRepo;
            _orderRepo = orderRepo;
            _uow = uow;
            _options = options.Value;
            _logger = logger;
            _orderNotifier = orderNotifier;
        }

        public async Task<ErrorOr<Payment>> CreatePaymentIntentAsync(
            Guid orderId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderRepo.FirstOrDefaultAsync(new OrderForPaymentProcessingSpec(orderId, userId), cancellationToken);
            if (order is null) return Errors.OrderErrors.NotFoundWithId(orderId.ToString());
            if (order.Status != OrderStatus.Pending) return Errors.PaymentErrors.Order_NotPayable();
            if (order.Payment is not null) return Errors.PaymentErrors.Order_HasExistingPayment();

            var service = new PaymentIntentService();
            try
            {
                var intent = await service.CreateAsync(new PaymentIntentCreateOptions
                {
                    // cents - multiply by 100
                    Amount = (long)(order.TotalPrice * 100),
                    Currency = "usd",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "order_id", orderId.ToString() },
                        { "user_id",  userId.ToString()  }
                    }
                }, cancellationToken: cancellationToken);

                var payment = Payment.Create(orderId, userId, order.TotalPrice, intent.Id, intent.ClientSecret);
                order.MarkAsAwaitingPayment();
                await _paymentRepo.AddAsync(payment, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "PaymentIntent created. OrderId: {OrderId}, IntentId: {IntentId}",
                    orderId, intent.Id);

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred when trying to create payment intent for order with info: order ID '{OrderId}' - user ID '{UserId}' as succeeded, detail: {ExMsg}",
                    orderId, userId, ex.Message);
                return Errors.PaymentErrors.Stripe_CreatePaymentIntentFailed();
            }
        }

        public async Task<ErrorOr<Payment>> HandleWebhookAsync(
            string json,
            string stripeSignature,
            CancellationToken cancellationToken = default)
        {
            await Task.Delay(2000);
            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _options.WebhookSecret, throwOnApiVersionMismatch: false);
            }
            catch (StripeException ex)
            {
                _logger.LogWarning("Invalid Stripe webhook signature: {Message}", ex.Message);
                return Errors.PaymentErrors.Stripe_InvalidWebhookSignature();
            }
            _logger.LogInformation("Stripe webhook received: {EventType}", stripeEvent.Type);
            return stripeEvent.Type switch
            {
                EventTypes.PaymentIntentSucceeded => await OnSucceededAsync((PaymentIntent)stripeEvent.Data.Object, cancellationToken),
                EventTypes.PaymentIntentPaymentFailed => await OnFailedAsync((PaymentIntent)stripeEvent.Data.Object, cancellationToken),
                EventTypes.PaymentIntentCanceled => await OnCancelledAsync((PaymentIntent)stripeEvent.Data.Object, cancellationToken),
                EventTypes.ChargeRefunded => await OnRefundedAsync((Charge)stripeEvent.Data.Object, cancellationToken),
                _ => Errors.PaymentErrors.Stripe_EventNotSupported()
            };
        }

        public async Task<ErrorOr<Payment>> RefundAsync(
            Guid paymentId,
            decimal? amount = null,
            CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepo.FirstOrDefaultAsync(new PaymentByIdSpec(paymentId), cancellationToken);
            if (payment is null) return Errors.PaymentErrors.NotFoundWithId(paymentId.ToString());
            if (payment.Status != PaymentStatus.Succeeded) return Errors.PaymentErrors.NotRefundable(payment.Status.ToString());

            var refundAmount = amount ?? payment.Amount;

            try
            {
                await new RefundService().CreateAsync(new RefundCreateOptions
                {
                    PaymentIntent = payment.StripePaymentIntentId,
                    Amount = (long)(refundAmount * 100)
                }, cancellationToken: cancellationToken);
                payment.MarkAsRefunded(refundAmount);
                payment.Order.MarkAsRefunded();
                await _uow.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Payment '{PaymentId}' refunded along with order ID '{OrderId}'. Amount: {Amount}", paymentId, payment.Order.Id, refundAmount);
                await _orderNotifier.NotifyOrderUpdated(payment.Order.Id, payment.Order.Status.ToString(), cancellationToken);
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred when trying to refund payment and purchase order with info: payment ID '{PaymentId}' - order ID '{OrderId}', detail: {ExMsg}",
                    paymentId, payment.OrderId, ex.Message);
                return Errors.PaymentErrors.Stripe_RefundedPaymentFailed();
            }
        }

        private async Task<ErrorOr<Payment>> OnSucceededAsync(
            PaymentIntent intent,
            CancellationToken cancellationToken)
        {
            var paymentResult = await FindByIntentIdAsync(intent.Id, cancellationToken);
            if (paymentResult.IsError) return paymentResult.FirstError;
            var payment = paymentResult.Value;

            if (payment.Status == PaymentStatus.Succeeded) return payment;

            try
            {
                payment.MarkAsSucceeded(intent.LatestChargeId);
                payment.Order.MarkAsPaid();

                await _uow.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Payment succeeded. OrderId: {OrderId}, ChargeId: {ChargeId}",
                    payment.OrderId, intent.LatestChargeId);
                await _orderNotifier.NotifyOrderUpdated(payment.Order.Id, payment.Order.Status.ToString(), cancellationToken);
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred when trying to mark payment with info: intent ID '{IntentId}' - order ID '{OrderId}' as succeeded, detail: {ExMsg}",
                    intent.Id, payment.OrderId, ex.Message);
                return Errors.PaymentErrors.Stripe_MarkPaymentAsSucceededFailed();
            }
        }

        private async Task<ErrorOr<Payment>> OnFailedAsync(PaymentIntent intent, CancellationToken cancellationToken)
        {
            var paymentResult = await FindByIntentIdAsync(intent.Id, cancellationToken);
            if (paymentResult.IsError) return paymentResult.FirstError;
            var payment = paymentResult.Value;

            try
            {
                payment.MarkAsFailed(intent.LastPaymentError?.Message);
                payment.Order.CancelOrder();

                await _uow.SaveChangesAsync(cancellationToken);
                _logger.LogWarning(
                    "Payment failed. OrderId: {OrderId}, Reason: {Reason}",
                    payment.OrderId, intent.LastPaymentError?.Message);
                await _orderNotifier.NotifyOrderUpdated(payment.Order.Id, payment.Order.Status.ToString(), cancellationToken);
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred when trying to mark payment with info: intent ID '{IntentId}' - order ID '{OrderId}' as failed, detail: {ExMsg}",
                    intent.Id, payment.OrderId, ex.Message);
                return Errors.PaymentErrors.Stripe_MarkPaymentAsFailedUnsuccessful();
            }
        }

        private async Task<ErrorOr<Payment>> OnCancelledAsync(PaymentIntent intent, CancellationToken cancellationToken)
        {
            var paymentResult = await FindByIntentIdAsync(intent.Id, cancellationToken);
            if (paymentResult.IsError) return paymentResult.FirstError;
            var payment = paymentResult.Value;

            // idempotent — Stripe may send the event more than once
            if (payment.Status == PaymentStatus.Cancelled)
            {
                _logger.LogInformation(
                    "Payment {PaymentId} already cancelled, skipping.", payment.Id);
                return payment;
            }

            try
            {
                payment.MarkAsCancelled();
                payment.Order.CancelOrder();

                await _uow.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Payment cancelled via webhook. OrderId: {OrderId}, Reason: {Reason}",
                    payment.OrderId, intent.CancellationReason);
                await _orderNotifier.NotifyOrderUpdated(payment.Order.Id, payment.Order.Status.ToString(), cancellationToken);
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred when trying to mark payment with info: intent ID '{IntentId}' - order ID '{OrderId}' as cancelled, detail: {ExMsg}",
                    intent.Id, payment.OrderId, ex.Message);
                return Errors.PaymentErrors.Stripe_MarkPaymentAsCancelledFailed();
            }
        }

        private async Task<ErrorOr<Payment>> OnRefundedAsync(Charge charge, CancellationToken cancellationToken)
        {
            var paymentResult = await FindByIntentIdAsync(charge.PaymentIntentId, cancellationToken);
            if (paymentResult.IsError) return paymentResult.FirstError;
            var payment = paymentResult.Value;

            if (payment.Status == PaymentStatus.Refunded)
            {
                _logger.LogInformation("Payment {PaymentId} already marked as refunded, skipping.", payment.Id);
                return payment;
            }

            try
            {
                decimal amount = charge.AmountRefunded / 100m;
                payment.MarkAsRefunded(amount);
                payment.Order.MarkAsRefunded();

                await _uow.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Payment refunded via Webhook. OrderId: {OrderId}, Amount: {Amount}",
                    payment.OrderId, amount);
                await _orderNotifier.NotifyOrderUpdated(payment.Order.Id, payment.Order.Status.ToString(), cancellationToken);
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred when trying to process refund webhook for PaymentIntent {IntentId}, detail: {ExMsg}", charge.PaymentIntentId, ex.Message);
                return Errors.PaymentErrors.Stripe_RefundedPaymentFailed();
            }
        }

        private async Task<ErrorOr<Payment>> FindByIntentIdAsync(string intentId, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepo.FirstOrDefaultAsync(new PaymentByIntentIdSpec(intentId), cancellationToken);
            if (payment is null) return Errors.PaymentErrors.Stripe_PaymentNotFoundWithIntentId(intentId);
            return payment;
        }

    }
}
