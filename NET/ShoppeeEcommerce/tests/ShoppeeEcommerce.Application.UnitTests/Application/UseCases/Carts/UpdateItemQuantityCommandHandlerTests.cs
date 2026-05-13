using ErrorOr;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.UpdateQuantity;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Carts
{
    public class UpdateItemQuantityCommandHandlerTests
    {
        private readonly Mock<ICartService> _mockCartService;
        private readonly UpdateItemQuantityCommandHandler _handler;

        public UpdateItemQuantityCommandHandlerTests()
        {
            _mockCartService = new Mock<ICartService>();
            _handler = new UpdateItemQuantityCommandHandler(_mockCartService.Object);
        }

        [Fact]
        public async Task Handle_ReturnsUpdated_WhenQuantityIsUpdatedSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var newQuantity = 5;
            var command = new UpdateItemQuantityCommand(productId, newQuantity);

            _mockCartService.Setup(x => x.UpdateQuantityAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify parameters were passed correctly
            _mockCartService.Verify(x => x.UpdateQuantityAsync(
                productId,
                newQuantity,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsUpdateItemQuantityFailed_WhenServiceThrowsException()
        {
            // Arrange
            var command = new UpdateItemQuantityCommand(Guid.NewGuid(), 1);
            var expectedError = Errors.CartErrors.UpdateItemQuantityFailed();
            _mockCartService.Setup(x => x.UpdateQuantityAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Concurrency error or connection failure"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }
    }
}
