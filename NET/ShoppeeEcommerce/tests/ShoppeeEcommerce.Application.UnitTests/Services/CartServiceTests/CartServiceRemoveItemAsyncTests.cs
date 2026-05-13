using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.Implementations.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.Tests.Services.CartServiceTests
{
    public class CartServiceRemoveItemAsyncTests
    {
        private readonly Mock<IRepository<Cart, Guid>> _mockCartRepo;
        private readonly Mock<ICartOwnerProvider> _mockOwnerProvider;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ILogger<CartService>> _mockLogger;
        private readonly Mock<IRepository<Product, Guid>> _mockProductRepo;
        private readonly CartService _cartService;
        public CartServiceRemoveItemAsyncTests()
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
        public async Task RemoveItemAsync_RemovesItemAndSaves_WhenProductIsInCart()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cart = new Cart { Id = Guid.NewGuid() };
            // Pre-populate the cart with the item to be removed
            cart.AddItem(productId, 1, 100m);

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid.NewGuid(), "session"));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            // Act
            await _cartService.RemoveItemAsync(productId);

            // Assert
            Assert.DoesNotContain(cart.CartItems, i => i.ProductId == productId);
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task RemoveItemAsync_DoesNothing_WhenProductIsNotInCart()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var differentProductId = Guid.NewGuid();
            var cart = new Cart();
            cart.AddItem(differentProductId, 1, 50m);

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, "session-123"));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            // Act
            await _cartService.RemoveItemAsync(productId);

            // Assert
            // The cart still contains the other product, and no error was thrown
            Assert.Single(cart.CartItems);
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveItemAsync_ThrowsAndLogs_WhenExceptionOccurs()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cart = new Cart();

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, "session-123"));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _mockUow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Concurrency violation"));

            // Act
            var act = () => _cartService.RemoveItemAsync(productId);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error when trying to remove product with ID '{productId}'")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
