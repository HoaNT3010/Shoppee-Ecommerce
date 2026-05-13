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
    public class StripePaymentServiceRefundAsyncTests
    {
        private readonly Mock<IRepository<Payment, Guid>> _mockPaymentRepo;
        private readonly Mock<IRepository<Order, Guid>> _mockOrderRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IStripeServiceWrapper> _mockStripeWrapper;
        private readonly Mock<IOrderRealtimeNotifier> _mockNotifier;
        private readonly Mock<ILogger<StripePaymentService>> _mockLogger;
        private readonly StripePaymentService _stripeService;
        public StripePaymentServiceRefundAsyncTests()
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

        [Fact]
        public async Task RefundAsync_ReturnsPayment_WhenFullRefundIsSuccessful()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var amount = 100m;
            var payment = CreateSucceededPayment(paymentId, amount);

            _mockPaymentRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<PaymentByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            _mockStripeWrapper.Setup(x => x.CreateRefundAsync(It.IsAny<RefundCreateOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Refund());

            // Act
            var result = await _stripeService.RefundAsync(paymentId);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(PaymentStatus.Refunded, payment.Status);
            Assert.Equal(OrderStatus.Refunded, payment.Order.Status);

            _mockStripeWrapper.Verify(x => x.CreateRefundAsync(
                It.Is<RefundCreateOptions>(o => o.Amount == (long)(amount * 100)),
                It.IsAny<CancellationToken>()), Times.Once);

            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockNotifier.Verify(x => x.NotifyOrderUpdated(payment.Order.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RefundAsync_ReturnsPayment_WhenPartialRefundIsRequested()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var fullAmount = 100m;
            var partialAmount = 40m;
            var payment = CreateSucceededPayment(paymentId, fullAmount);

            _mockPaymentRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<PaymentByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            // Act
            var result = await _stripeService.RefundAsync(paymentId, partialAmount);

            // Assert
            _mockStripeWrapper.Verify(x => x.CreateRefundAsync(
                It.Is<RefundCreateOptions>(o => o.Amount == (long)(partialAmount * 100)),
                It.IsAny<CancellationToken>()), Times.Once);
            Assert.False(result.IsError);
        }

        [Fact]
        public async Task RefundAsync_ReturnsNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var expectedError = Errors.PaymentErrors.NotFoundWithId(paymentId.ToString());
            _mockPaymentRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<PaymentByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Payment?)null);

            // Act
            var result = await _stripeService.RefundAsync(paymentId);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        [Fact]
        public async Task RefundAsync_ReturnsNotRefundable_WhenPaymentStatusIsNotSucceeded()
        {
            // Arrange
            var payment = CreateSucceededPayment(Guid.NewGuid(), 100m);
            payment.Status = PaymentStatus.Failed;
            var expectedError = Errors.PaymentErrors.NotRefundable(payment.Status.ToString());

            _mockPaymentRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<PaymentByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            // Act
            var result = await _stripeService.RefundAsync(payment.Id);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        [Fact]
        public async Task RefundAsync_ReturnsStripeFailure_WhenWrapperThrowsException()
        {
            // Arrange
            var payment = CreateSucceededPayment(Guid.NewGuid(), 100m);
            var expectedError = Errors.PaymentErrors.Stripe_RefundedPaymentFailed();
            _mockPaymentRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<PaymentByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(payment);

            _mockStripeWrapper.Setup(x => x.CreateRefundAsync(It.IsAny<RefundCreateOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Stripe connection error"));

            // Act
            var result = await _stripeService.RefundAsync(payment.Id);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
            _mockLogger.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        private Payment CreateSucceededPayment(Guid id, decimal amount)
        {
            // Adjust this to your actual factory/constructor
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var payment = Payment.Create(orderId, userId, amount, "pi_test", "secret");
            payment.Order = new Order();

            // Ensure the order is in a state that allows marking as refunded
            payment.Order.MarkAsPaid();
            payment.MarkAsSucceeded("ch_test");

            return payment;
        }
    }
}
