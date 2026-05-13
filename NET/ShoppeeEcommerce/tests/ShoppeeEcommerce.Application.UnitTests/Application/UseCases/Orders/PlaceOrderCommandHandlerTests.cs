using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Orders.Place;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Orders
{
    public class PlaceOrderCommandHandlerTests
        : CommandHandlerTestBase<Order, Guid>
    {
        readonly PlaceOrderCommandHandler _handler;
        readonly Mock<ILogger<PlaceOrderCommandHandler>> _loggerMock;
        readonly Mock<IRepository<Product, Guid>> _productRepoMock;
        readonly Mock<ICartService> _cartServiceMock;
        public PlaceOrderCommandHandlerTests()
        {
            _loggerMock = CreateLoggerMock<PlaceOrderCommandHandler>();
            _productRepoMock = new Mock<IRepository<Product, Guid>>();
            _cartServiceMock = new Mock<ICartService>();
            _handler = new PlaceOrderCommandHandler(
                RepoMock.Object,
                _productRepoMock.Object,
                _cartServiceMock.Object,
                Uow,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsCreated_WhenCartIsValidAndProductsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var cart = CreateTestCart(userId, productId, quantity: 2);
            var product = CreateTestProduct(productId, "Test Product", 50m);

            _cartServiceMock.Setup(x => x.GetUserCart(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _productRepoMock.Setup(x => x.ListAsync(It.IsAny<ProductsByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            var command = new PlaceOrderCommand(userId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Created, result.Value);

            // Verify mapping logic
            RepoMock.Verify(x => x.AddAsync(It.Is<Order>(o =>
                o.UserId == userId &&
                o.Items.Count == 1 &&
                o.TotalPrice == 100m), // 50 * 2
                It.IsAny<CancellationToken>()), Times.Once);

            // Verify side effect (Cart emptied)
            Assert.Empty(cart.CartItems);
            UoWMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_ReturnsEmptyCart_WhenCartHasNoItems()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var emptyCart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
            var expectedError = Errors.OrderErrors.EmptyCart();

            _cartServiceMock.Setup(x => x.GetUserCart(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyCart);

            // Act
            var result = await _handler.Handle(new PlaceOrderCommand(userId), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        [Fact]
        public async Task Handle_ReturnsContainsInvalidProducts_WhenProductIsDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cart = CreateTestCart(userId, productId, 1);
            var deletedProduct = CreateTestProduct(productId, "Deleted", 10m);
            deletedProduct.IsDeleted = true;
            var expectedError = Errors.OrderErrors.ContainsInvalidProducts();

            _cartServiceMock.Setup(x => x.GetUserCart(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            _productRepoMock.Setup(x => x.ListAsync(It.IsAny<ProductsByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { deletedProduct });

            // Act
            var result = await _handler.Handle(new PlaceOrderCommand(userId), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        [Fact]
        public async Task Handle_ReturnsCreateOrderFailed_WhenOrderRepoThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var cart = CreateTestCart(userId, productId, 1);
            var product = CreateTestProduct(productId, "Fail", 10m);
            var expectedError = Errors.OrderErrors.CreateOrderFailed();

            _cartServiceMock.Setup(x => x.GetUserCart(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);
            _productRepoMock.Setup(x => x.ListAsync(It.IsAny<ProductsByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            RepoMock.Setup(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Persistence failed"));

            // Act
            var result = await _handler.Handle(new PlaceOrderCommand(userId), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
            VerifyErrorLog(_loggerMock, Times.Once());
        }

        [Fact]
        public async Task Handler_ReturnsNoCartInfo_WhenCartNotFound()
        {
            // Arrange
            _cartServiceMock.Setup(x => x.GetUserCart(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Cart?)null);
            var expectedError = Errors.OrderErrors.NoCartInfo();

            // Act
            var result = await _handler.Handle(new PlaceOrderCommand(Guid.NewGuid()), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        [Fact]
        public async Task Handler_ReturnsGuestCartNotAllowed_WhenCartCheckoutWithGuestCart()
        {
            // Arrange
            _cartServiceMock.Setup(x => x.GetUserCart(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Cart() { SessionId = "session-123", UserId = null });
            var expectedError = Errors.OrderErrors.GuestCartNotAllowed();

            // Act
            var result = await _handler.Handle(new PlaceOrderCommand(Guid.NewGuid()), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        [Fact]
        public async Task Handler_ReturnsMismatchItem_WhenItemCountNotMatchProductCount()
        {
            // Arrange
            var cart = CreateTestCart(Guid.NewGuid(), Guid.NewGuid(), 1);
            _cartServiceMock.Setup(x => x.GetUserCart(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);
            _productRepoMock.Setup(x => x.ListAsync(It.IsAny<ProductsByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());
            var expectedError = Errors.OrderErrors.ItemCountNotMatch();

            // Act
            var result = await _handler.Handle(new PlaceOrderCommand(Guid.NewGuid()), CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(expectedError, result.FirstError);
        }

        private Cart CreateTestCart(Guid userId, Guid productId, int quantity)
        {
            return new Cart
            {
                UserId = userId,
                CartItems = new List<CartItem>
                {
                    new() { ProductId = productId, Quantity = quantity }
                }
            };
        }

        private Product CreateTestProduct(Guid id, string name, decimal price)
        {
            return new Product
            {
                Id = id,
                Name = name,
                Price = price,
                Status = ProductStatus.Published,
                IsDeleted = false
            };
        }
    }
}
