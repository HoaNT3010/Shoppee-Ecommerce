using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.Implementations.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.Tests.Services.CartServiceTests
{
    public class CartServiceAddItemAsyncTests
    {
        private readonly Mock<IRepository<Cart, Guid>> _mockCartRepo;
        private readonly Mock<ICartOwnerProvider> _mockOwnerProvider;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ILogger<CartService>> _mockLogger;
        private readonly Mock<IRepository<Product, Guid>> _mockProductRepo;
        private readonly CartService _cartService;
        public CartServiceAddItemAsyncTests()
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
        public async Task AddItemAsync_Succeeds_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var quantity = 2;
            var price = 100m;
            var cart = new Cart { Id = Guid.NewGuid() };
            var product = new Product { Id = productId, Price = price };

            // Mocking internal GetOrCreateCartAsync requirements
            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid.NewGuid(), "session"));
            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            // Mocking product retrieval
            _mockProductRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<PublicProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            await _cartService.AddItemAsync(productId, quantity);

            // Assert
            // 2 SaveChanges 1 for merging carts (both user and session carts exist) and 1 for adding item
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            Assert.Contains(cart.CartItems, i => i.ProductId == productId && i.Quantity == quantity);
        }

        [Fact]
        public async Task AddItemAsync_ThrowsArgumentNullException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cart = new Cart();

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, "session"));
            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _mockProductRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<PublicProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            // Act
            var act = () => _cartService.AddItemAsync(productId, 1);

            // Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(act);
            Assert.Contains($"Failed to find the corresponding product with ID '{productId}'", exception.Message);
        }

        [Fact]
        public async Task AddItemAsync_ThrowsAndLogs_WhenDatabaseExceptionOccurs()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Price = 50 };
            var cart = new Cart();

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, "session"));
            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);
            _mockProductRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<PublicProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _mockUow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Deadlock"));

            // Act
            var act = () => _cartService.AddItemAsync(productId, 1);

            // Assert
            await Assert.ThrowsAsync<Exception>(act);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error when trying to add new product to cart")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
