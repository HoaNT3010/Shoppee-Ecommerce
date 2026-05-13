using ErrorOr;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.Clear;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Carts
{
    public class ClearCartCommandHandlerTests
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly ClearCartCommandHandler _handler;

        public ClearCartCommandHandlerTests()
        {
            _mockCartService = new Mock<ICartService>();
            _handler = new ClearCartCommandHandler(_mockCartService.Object);
        }

        [Fact]
        public async Task Handle_ReturnsUpdated_WhenCartIsClearedSuccessfully()
        {
            // Arrange
            var command = new ClearCartCommand();
            _mockCartService.Setup(x => x.ClearAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);
            _mockCartService.Verify(x => x.ClearAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsClearItemsFailed_WhenServiceThrowsException()
        {
            // Arrange
            var command = new ClearCartCommand();
            _mockCartService.Setup(x => x.ClearAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB connection failed"));
            var expectedError = Errors.CartErrors.ClearItemsFailed();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }
    }
}
