using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Realtime;
using ShoppeeEcommerce.Application.UseCases.Orders.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.Infrastructure.Payments.Stripe;
using Stripe;

namespace ShoppeeEcommerce.Infrastructure.Tests.Payments.Stripe
{
    public class StripePaymentServiceCreatePaymentIntentAsyncTests
    {
        private readonly Mock<IRepository<Payment, Guid>> _mockPaymentRepo;
        private readonly Mock<IRepository<Order, Guid>> _mockOrderRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IStripeServiceWrapper> _mockStripeWrapper;
        private readonly Mock<IOrderRealtimeNotifier> _mockNotifier;
        private readonly Mock<ILogger<StripePaymentService>> _mockLogger;
        private readonly StripePaymentService _stripeService;

        public StripePaymentServiceCreatePaymentIntentAsyncTests()
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
        public async Task CreatePaymentIntentAsync_ReturnsPayment_WhenOrderIsValidAndStripeSucceeds()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = CreateTestOrder(orderId, userId, 100m, OrderStatus.Pending);

            var stripeIntent = new PaymentIntent
            {
                Id = "pi_test_123",
                ClientSecret = "secret_123"
            };

            _mockOrderRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<OrderForPaymentProcessingSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _mockStripeWrapper.Setup(x => x.CreatePaymentIntentAsync(It.IsAny<PaymentIntentCreateOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(stripeIntent);

            // Act
            var result = await _stripeService.CreatePaymentIntentAsync(orderId, userId);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(stripeIntent.Id, result.Value.StripePaymentIntentId);
            Assert.Equal(OrderStatus.AwaitingPayment, order.Status);
            _mockPaymentRepo.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreatePaymentIntentAsync_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = Guid.NewGuid().ToString();
            var expectedError = Errors.OrderErrors.NotFoundWithId(orderId);
            _mockOrderRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<OrderForPaymentProcessingSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            // Act
            var result = await _stripeService.CreatePaymentIntentAsync(Guid.Parse(orderId), Guid.NewGuid());

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        [Fact]
        public async Task CreatePaymentIntentAsync_ReturnsNotPayable_WhenOrderStatusIsNotPending()
        {
            // Arrange
            var expectedError = Errors.PaymentErrors.Order_NotPayable();
            var order = CreateTestOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, OrderStatus.Cancelled);

            _mockOrderRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<OrderForPaymentProcessingSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = await _stripeService.CreatePaymentIntentAsync(order.Id, Guid.NewGuid());

            // Assert
            Assert.True(result.IsError);
            var error = result.Errors.First();
            Assert.Equal(expectedError, error);
        }

        [Fact]
        public async Task CreatePaymentIntentAsync_ReturnsStripeFailure_WhenStripeThrowsException()
        {
            // Arrange
            var expectedError = Errors.PaymentErrors.Stripe_CreatePaymentIntentFailed();
            var order = CreateTestOrder(Guid.NewGuid(), Guid.NewGuid(), 100m, OrderStatus.Pending);

            _mockOrderRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<OrderForPaymentProcessingSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _mockStripeWrapper.Setup(x => x.CreatePaymentIntentAsync(It.IsAny<PaymentIntentCreateOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Stripe API Down"));

            // Act
            var result = await _stripeService.CreatePaymentIntentAsync(order.Id, Guid.NewGuid());

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
            _mockLogger.Verify(
                x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // Helper method to create a valid Order entity
        private Order CreateTestOrder(Guid id, Guid userId, decimal price, OrderStatus status)
        {
            var order = Order.CreateNewOrder(userId);
            order.Id = id;
            order.TotalPrice = price;
            order.Status = status;
            return order;
        }
    }
}
