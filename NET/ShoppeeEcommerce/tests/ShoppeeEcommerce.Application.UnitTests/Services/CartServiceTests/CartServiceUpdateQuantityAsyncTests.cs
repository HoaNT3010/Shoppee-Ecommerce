using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.Implementations.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.Tests.Services.CartServiceTests
{
    public class CartServiceUpdateQuantityAsyncTests
    {
        private readonly Mock<IRepository<Cart, Guid>> _mockCartRepo;
        private readonly Mock<ICartOwnerProvider> _mockOwnerProvider;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ILogger<CartService>> _mockLogger;
        private readonly Mock<IRepository<Product, Guid>> _mockProductRepo;
        private readonly CartService _cartService;
        public CartServiceUpdateQuantityAsyncTests()
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
        public async Task UpdateQuantityAsync_UpdatesValueAndSaves_WhenProductExistsInCart()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var initialQuantity = 1;
            var newQuantity = 5;
            var cart = new Cart { Id = Guid.NewGuid() };
            cart.AddItem(productId, initialQuantity, 10.0m);

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid.NewGuid(), "session-123"));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            // Act
            await _cartService.UpdateQuantityAsync(productId, newQuantity);

            // Assert
            var item = Assert.Single(cart.CartItems);
            Assert.Equal(newQuantity, item.Quantity);
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateQuantityAsync_ThrowsAndLogs_WhenExceptionOccursDuringSave()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cart = new Cart();
            cart.AddItem(productId, 1, 10.0m);

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, "session-123"));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _mockUow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var act = () => _cartService.UpdateQuantityAsync(productId, 10);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error when trying to update quantity of product with ID '{productId}'")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateQuantityAsync_Completes_WhenProductDoesNotExistInCart()
        {
            // Arrange
            var productIdInCart = Guid.NewGuid();
            var productIdToUpdate = Guid.NewGuid();
            var cart = new Cart();
            cart.AddItem(productIdInCart, 1, 10.0m);

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid.NewGuid(), null));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            // Act
            await _cartService.UpdateQuantityAsync(productIdToUpdate, 10);

            // Assert
            // Verify quantity of the existing item didn't change and the call still attempted to save
            Assert.Equal(1, cart.CartItems.First().Quantity);
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
