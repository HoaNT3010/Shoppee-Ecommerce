using ErrorOr;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.AddItem;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Carts
{
    public class AddCartItemCommandHandlerTests
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly AddCartItemCommandHandler _handler;

        public AddCartItemCommandHandlerTests()
        {
            _mockCartService = new Mock<ICartService>();
            _handler = new AddCartItemCommandHandler(_mockCartService.Object);
        }

        [Fact]
        public async Task Handle_ReturnsCreated_WhenItemIsAddedSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var quantity = 2;
            var command = new AddCartItemCommand(productId, quantity);

            _mockCartService.Setup(x => x.AddItemAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Created, result.Value);
            _mockCartService.Verify(x => x.AddItemAsync(
                productId,
                quantity,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsAddItemFailed_WhenServiceThrowsException()
        {
            // Arrange
            var command = new AddCartItemCommand(Guid.NewGuid(), 1);
            var expectedError = Errors.CartErrors.AddItemFailed();
            _mockCartService.Setup(x => x.AddItemAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("External service error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }
    }
}
