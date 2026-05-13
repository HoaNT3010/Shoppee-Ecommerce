using ErrorOr;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.DeleteItem;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Carts
{
    public class DeleteCartItemCommandHandlerTests
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly DeleteCartItemCommandHandler _handler;

        public DeleteCartItemCommandHandlerTests()
        {
            _mockCartService = new Mock<ICartService>();
            _handler = new DeleteCartItemCommandHandler(_mockCartService.Object);
        }

        [Fact]
        public async Task Handle_ReturnsDeleted_WhenItemIsRemovedSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new DeleteCartItemCommand(productId);

            _mockCartService.Setup(x => x.RemoveItemAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Deleted, result.Value);
            _mockCartService.Verify(x => x.RemoveItemAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsRemoveItemFailed_WhenServiceThrowsException()
        {
            // Arrange
            var command = new DeleteCartItemCommand(Guid.NewGuid());
            var expectedError = Errors.CartErrors.RemoveItemFailed();
            _mockCartService.Setup(x => x.RemoveItemAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Infrastructure failure"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }
    }
}
