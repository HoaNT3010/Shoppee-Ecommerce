using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Realtime;
using ShoppeeEcommerce.Application.UseCases.Payments.Common.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.Infrastructure.Payments.Stripe;
using Stripe;

namespace ShoppeeEcommerce.Infrastructure.Tests.Payments.Stripe
{
    public class StripePaymentServiceHandleWebhookAsyncTests
    {
        private readonly Mock<IRepository<Payment, Guid>> _mockPaymentRepo;
        private readonly Mock<IRepository<Order, Guid>> _mockOrderRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IStripeServiceWrapper> _mockStripeWrapper;
        private readonly Mock<IOrderRealtimeNotifier> _mockNotifier;
        private readonly Mock<ILogger<StripePaymentService>> _mockLogger;
        private readonly StripePaymentService _stripeService;
        public StripePaymentServiceHandleWebhookAsyncTests()
        {
            _mockPaymentRepo = new Mock<IRepository<Payment, Guid>>();
            _mockOrderRepo = new Mock<IRepository<Order, Guid>>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockStripeWrapper = new Mock<IStripeServiceWrapper>();
            _mockNotifier = new Mock<IOrderRealtimeNotifier>();
            _mockLogger = new Mock<ILogger<StripePaymentService>>();

            var options = Options.Create(new StripeOptions { WebhookSecret = "test_secret" });

            _stripeService = new StripePaymentService(
                _mockPaymentRepo.Object,
                _mockOrderRepo.Object,
                _mockUow.Object,
                options,
                _mockLogger.Object,
                _mockNotifier.Object,
                _mockStripeWrapper.Object);
        }

        private Event CreateStripeEvent(string type, IHasObject dataObject)
        {
            return new Event
            {
                Type = type,
                Data = new EventData { Object = dataObject }
            };
        }

        private Payment CreatePendingPayment(string intentId)
        {
            var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), 100m, intentId, "secret");
            payment.Order = new Order();
            payment.Order.MarkAsAwaitingPayment();
            return payment;
        }

        [Fact]
        public async Task HandleWebhookAsync_ReturnsInvalidSignature_WhenSignatureIsInvalid()
        {
            // Arrange
            var json = "{}";
            var signature = "invalid_sig";
            var expectedError = Errors.PaymentErrors.Stripe_InvalidWebhookSignature();
            _mockStripeWrapper.Setup(x => x.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new StripeException("Invalid signature"));

            // Act
            var result = await _stripeService.HandleWebhookAsync(json, signature);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
            _mockLogger.Verify(
                x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleWebhookAsync_ReturnsEventNotSupported_WhenEventTypeIsUnknown()
        {
            // Arrange
            var json = "{}";
            var stripeEvent = new Event { Type = "unknown.event" };
            var expectedError = Errors.PaymentErrors.Stripe_EventNotSupported();
            _mockStripeWrapper.Setup(x => x.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stripeEvent);

            // Act
            var result = await _stripeService.HandleWebhookAsync(json, "sig");

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        [Fact]
        public async Task HandleWebhookAsync_UpdatesStatusToSucceeded_WhenPaymentIntentSucceededReceived()
        {
            // Arrange
            var intentId = "pi_success_123";
            var payment = CreatePendingPayment(intentId);

            // Mock the Wrapper to return a "Succeeded" event
            var stripeEvent = CreateStripeEvent(EventTypes.PaymentIntentSucceeded, new PaymentIntent
            {
                Id = intentId,
                LatestChargeId = "ch_123"
            });

            _mockStripeWrapper.Setup(w => w.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stripeEvent);

            _mockPaymentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<PaymentByIntentIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            // Act
            var result = await _stripeService.HandleWebhookAsync("json", "sig");

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(PaymentStatus.Succeeded, payment.Status);
            Assert.Equal(OrderStatus.Paid, payment.Order.Status);

            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockNotifier.Verify(n => n.NotifyOrderUpdated(payment.Order.Id, "Paid", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleWebhookAsync_ReturnsError_WhenFailedToUpdateOnSucceeded()
        {
            // Arrange
            var intentId = "pi_success_123";
            var payment = CreatePendingPayment(intentId);
            var expectedError = Errors.PaymentErrors.Stripe_MarkPaymentAsSucceededFailed();
            var stripeEvent = CreateStripeEvent(EventTypes.PaymentIntentSucceeded, new PaymentIntent
            {
                Id = intentId,
                LatestChargeId = "ch_123"
            });

            _mockStripeWrapper.Setup(w => w.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stripeEvent);

            _mockPaymentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<PaymentByIntentIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB failure"));

            // Act
            var result = await _stripeService.HandleWebhookAsync("json", "sig");

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockNotifier.Verify(n => n.NotifyOrderUpdated(payment.Order.Id, "Paid", It.IsAny<CancellationToken>()), Times.Never);
            _mockLogger.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        [Fact]
        public async Task HandleWebhookAsync_CancelsOrder_WhenPaymentIntentPaymentFailedReceived()
        {
            // Arrange
            var intentId = "pi_failed_123";
            var payment = CreatePendingPayment(intentId);

            var stripeEvent = CreateStripeEvent(EventTypes.PaymentIntentPaymentFailed, new PaymentIntent
            {
                Id = intentId,
                LastPaymentError = new StripeError { Message = "Insufficient Funds" }
            });

            _mockStripeWrapper.Setup(w => w.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stripeEvent);

            _mockPaymentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<PaymentByIntentIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            // Act
            var result = await _stripeService.HandleWebhookAsync("json", "sig");

            // Assert
            Assert.Equal(PaymentStatus.Failed, payment.Status);
            Assert.Equal(OrderStatus.Cancelled, payment.Order.Status);
            _mockLogger.Verify(l => l.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task HandleWebhookAsync_ReturnError_WhenFailToUpdateOnFailed()
        {
            // Arrange
            var expectedError = Errors.PaymentErrors.Stripe_MarkPaymentAsFailedUnsuccessful();
            var intentId = "pi_failed_123";
            var payment = CreatePendingPayment(intentId);

            var stripeEvent = CreateStripeEvent(EventTypes.PaymentIntentPaymentFailed, new PaymentIntent
            {
                Id = intentId,
                LastPaymentError = new StripeError { Message = "Insufficient Funds" }
            });

            _mockStripeWrapper.Setup(w => w.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stripeEvent);

            _mockPaymentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<PaymentByIntentIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            _mockUow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB failure"));

            // Act
            var result = await _stripeService.HandleWebhookAsync("json", "sig");

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockLogger.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleWebhookAsync_IsIdempotent_WhenPaymentIsAlreadySucceeded()
        {
            // Arrange
            var intentId = "pi_already_done";
            var payment = CreatePendingPayment(intentId);
            payment.MarkAsSucceeded("ch_existing");

            var stripeEvent = CreateStripeEvent(EventTypes.PaymentIntentSucceeded, new PaymentIntent { Id = intentId });

            _mockStripeWrapper.Setup(w => w.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stripeEvent);

            _mockPaymentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<PaymentByIntentIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            // Act
            var result = await _stripeService.HandleWebhookAsync("json", "sig");

            // Assert
            // Verify SaveChanges was NOT called again because of the status check guard
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task HandleWebhookAsync_UpdatesToRefunded_WhenChargeRefundedReceived()
        {
            // Arrange
            var intentId = "pi_refund_123";
            var payment = CreatePendingPayment(intentId);
            payment.MarkAsSucceeded("ch_123");

            var stripeEvent = CreateStripeEvent(EventTypes.ChargeRefunded, new Charge
            {
                PaymentIntentId = intentId,
                AmountRefunded = 10000
            });

            _mockStripeWrapper.Setup(w => w.ConstructEvent(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(stripeEvent);

            _mockPaymentRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<PaymentByIntentIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            // Act
            var result = await _stripeService.HandleWebhookAsync("json", "sig");

            // Assert
            Assert.Equal(PaymentStatus.Refunded, payment.Status);
            Assert.Equal(100m, payment.RefundedAmount);
        }
    }
}
