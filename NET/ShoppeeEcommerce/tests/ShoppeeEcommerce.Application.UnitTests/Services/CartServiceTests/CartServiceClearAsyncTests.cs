using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.Implementations.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.Tests.Services.CartServiceTests
{
    public class CartServiceClearAsyncTests
    {
        private readonly Mock<IRepository<Cart, Guid>> _mockCartRepo;
        private readonly Mock<ICartOwnerProvider> _mockOwnerProvider;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ILogger<CartService>> _mockLogger;
        private readonly Mock<IRepository<Product, Guid>> _mockProductRepo;
        private readonly CartService _cartService;
        public CartServiceClearAsyncTests()
        {
            _mockCartRepo = new Mock<IRepository<Cart, Guid>>();
            _mockOwnerProvider = new Mock<ICartOwnerProvider>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<CartService>>();
            _mockProductRepo = new Mock<IRepository<Product, Guid>>();

            _cartService = new CartService(
                _mockCartRepo.Object,
                _mockOwnerProvider.Object,
                _mockUow.Object,
                _mockLogger.Object,
                _mockProductRepo.Object);
        }

        [Fact]
        public async Task ClearAsync_RemovesAllItemsAndSaves_WhenItemsExist()
        {
            // Arrange
            var cart = new Cart { Id = Guid.NewGuid() };
            cart.AddItem(Guid.NewGuid(), 1, 10.0m);
            cart.AddItem(Guid.NewGuid(), 2, 20.0m);

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid.NewGuid(), null));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            // Act
            await _cartService.ClearAsync();

            // Assert
            Assert.Empty(cart.CartItems);
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ClearAsync_ReturnsImmediately_WhenCartIsEmpty()
        {
            // Arrange
            var cart = new Cart { Id = Guid.NewGuid() }; // No items added

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid.NewGuid(), null));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            // Act
            await _cartService.ClearAsync();

            // Assert
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ClearAsync_ThrowsAndLogs_WhenExceptionOccurs()
        {
            // Arrange
            var cart = new Cart { Id = Guid.NewGuid() };
            cart.AddItem(Guid.NewGuid(), 1, 10.0m);

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, "session-123"));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _mockUow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database timeout"));

            // Act
            var act = () => _cartService.ClearAsync();

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error when trying to clear cart")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
