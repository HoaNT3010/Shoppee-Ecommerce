using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.Implementations.Services;
using ShoppeeEcommerce.Application.UseCases.Carts.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.Tests.Services.CartServiceTests
{
    public class CartServiceGetOrCreateCartAsyncTests
    {
        private readonly Mock<IRepository<Cart, Guid>> _mockCartRepo;
        private readonly Mock<ICartOwnerProvider> _mockOwnerProvider;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ILogger<CartService>> _mockLogger;
        private readonly Mock<IRepository<Product, Guid>> _mockProductRepo;
        private readonly CartService _cartService;

        public CartServiceGetOrCreateCartAsyncTests()
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
        public async Task GetOrCreateCartAsync_ReturnsNewCart_WhenNoCartsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sessionId = "session-123";
            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((userId, sessionId));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);

            // Act
            var result = await _cartService.GetOrCreateCartAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(sessionId, result.SessionId);
            _mockCartRepo.Verify(x => x.AddAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrCreateCartAsync_ReturnsMergedCart_WhenBothUserAndGuestCartsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sessionId = "session-123";
            var userCart = new Cart { UserId = userId };
            var guestCart = new Cart { SessionId = sessionId };

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((userId, sessionId));

            // Setup: first call returns userCart (for userId spec), second returns guestCart (for sessionId spec)
            _mockCartRepo.SetupSequence(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userCart)
                .ReturnsAsync(guestCart);

            // Act
            var result = await _cartService.GetOrCreateCartAsync();

            // Assert
            Assert.Equal(userCart, result);
            _mockCartRepo.Verify(x => x.Delete(guestCart), Times.Once);
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrCreateCartAsync_ReturnsMappedGuestCart_WhenOnlyGuestCartExistsAndUserIsAuthenticated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sessionId = "session-123";
            var guestCart = new Cart { SessionId = sessionId };

            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((userId, sessionId));

            _mockCartRepo.SetupSequence(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null) // No user cart
                .ReturnsAsync(guestCart);  // Found guest cart

            // Act
            var result = await _cartService.GetOrCreateCartAsync();

            // Assert
            Assert.Equal(guestCart, result);
            Assert.Equal(userId, guestCart.UserId);
            _mockUow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetOrCreateCartAsync_ThrowsAndLogs_WhenExceptionOccursDuringSave()
        {
            // Arrange
            _mockOwnerProvider.Setup(x => x.GetOwnerAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((null, "session-123"));

            _mockCartRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<CartWithItemsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);

            _mockUow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB Error"));

            // Act

            // Assert
            await Assert.ThrowsAsync<Exception>(() => _cartService.GetOrCreateCartAsync());
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error when trying to create new cart")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
