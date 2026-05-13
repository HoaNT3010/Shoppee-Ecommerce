using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Orders.Cancel;
using ShoppeeEcommerce.Application.UseCases.Orders.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Orders
{
    public class CancelOrderCommandHandlerTests
        : CommandHandlerTestBase<Order, Guid>
    {
        private readonly Mock<ILogger<CancelOrderCommandHandler>> _loggerMock;
        private readonly CancelOrderCommandHandler _handler;
        public CancelOrderCommandHandlerTests()
        {
            _loggerMock = CreateLoggerMock<CancelOrderCommandHandler>();
            _handler = new CancelOrderCommandHandler(RepoMock.Object, Uow, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsUpdated_WhenOrderIsSuccessfullyCancelled()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var orderId = Guid.NewGuid();
            var order = CreateTestOrder(orderId, userId, OrderStatus.Pending);
            var command = new CancelOrderCommand(orderId, userId);

            RepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<OrderByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(OrderStatus.Cancelled, order.Status);

            UoWMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsUpdated_WhenOrderIsAlreadyCancelled_Idempotency()
        {
            // Arrange
            var order = CreateTestOrder(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Cancelled);
            var command = new CancelOrderCommand(order.Id, order.UserId);

            RepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<OrderByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            UoWMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnsUserNotMatch_WhenUserIdDoesNotMatchOwner()
        {
            // Arrange
            var order = CreateTestOrder(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending);
            var command = new CancelOrderCommand(order.Id, Guid.NewGuid());
            var expectedError = Errors.OrderErrors.UserNotMatchOwner();

            RepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<OrderByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        [Fact]
        public async Task Handle_ReturnsCancelOrderFailed_WhenExceptionOccursDuringSave()
        {
            // Arrange
            var order = CreateTestOrder(Guid.NewGuid(), Guid.NewGuid(), OrderStatus.Pending);
            var command = new CancelOrderCommand(order.Id, order.UserId);
            var expectedError = Errors.OrderErrors.CancelOrderFailed();
            RepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<OrderByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            UoWMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database Error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
            VerifyErrorLog(_loggerMock, Times.Once());
        }

        private Order CreateTestOrder(Guid id, Guid userId, OrderStatus status)
        {
            var order = Order.CreateNewOrder(userId);
            order.Id = id;
            order.Status = status;

            if (status == OrderStatus.Cancelled) order.CancelOrder();
            if (status == OrderStatus.Paid) order.MarkAsPaid();

            return order;
        }
    }
}
