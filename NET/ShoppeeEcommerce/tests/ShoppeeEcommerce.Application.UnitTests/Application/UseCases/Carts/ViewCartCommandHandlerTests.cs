using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Carts.View;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.View;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Carts
{
    public class ViewCartCommandHandlerTests
        : HandlerTestBase<Product, Guid>
    {
        readonly Mock<ICartService> _cartServiceMock;
        readonly Mock<IUnitOfWork> _uowMock;
        readonly ViewCartCommandHandler _handler;
        public ViewCartCommandHandlerTests()
        {
            _cartServiceMock = new Mock<ICartService>();
            _uowMock = new Mock<IUnitOfWork>();

            _handler = new ViewCartCommandHandler(
                RepoMock.Object,
                _uowMock.Object,
                _cartServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsAvailableItem_WhenProductMatchesCartSnapshot()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var price = 100m;

            var cart = CreateTestCart(productId, price);
            var product = CreateTestProduct(productId, price, isPublished: true);

            _cartServiceMock.Setup(x => x.GetCartAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            RepoMock.Setup(x => x.ListAsync(It.IsAny<ProductsByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([product]);

            // Act
            var result = await _handler.Handle(new ViewCartCommand(), CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            var item = result.Value.Items.Single();
            Assert.Equal(CartItemStatus.Available, item.Status);
            Assert.Null(item.OldPrice);
        }

        [Fact]
        public async Task Handle_ReturnsPriceChangedStatus_WhenLivePriceDiffersFromSnapshot()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var snapshotPrice = 100m;
            var livePrice = 120m;

            var cart = CreateTestCart(productId, snapshotPrice);
            var product = CreateTestProduct(productId, livePrice, isPublished: true);

            _cartServiceMock.Setup(x => x.GetCartAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            RepoMock.Setup(x => x.ListAsync(It.IsAny<ProductsByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            // Act
            var result = await _handler.Handle(new ViewCartCommand(), CancellationToken.None);

            // Assert
            var item = result.Value.Items.Single();
            Assert.Equal(CartItemStatus.PriceChanged, item.Status);
            Assert.Equal(snapshotPrice, item.OldPrice);
            Assert.True(result.Value.HasPriceChangedItem);
        }

        [Fact]
        public async Task Handle_ReturnsInactiveStatus_WhenProductIsUnpublished()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cart = CreateTestCart(productId, 100m);
            var product = CreateTestProduct(productId, 100m, isPublished: false);

            _cartServiceMock.Setup(x => x.GetCartAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            RepoMock.Setup(x => x.ListAsync(It.IsAny<ProductsByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product> { product });

            // Act
            var result = await _handler.Handle(new ViewCartCommand(), CancellationToken.None);

            // Assert
            var item = result.Value.Items.Single();
            Assert.Equal(CartItemStatus.Inactive, item.Status);
            Assert.True(result.Value.HasInactiveItem);
        }

        [Fact]
        public async Task Handle_RemovesItemFromCart_WhenProductNoLongerExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var cart = CreateTestCart(productId, 100m);

            _cartServiceMock.Setup(x => x.GetCartAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(cart);

            // Return empty list of products (simulating deleted product)
            RepoMock.Setup(x => x.ListAsync(It.IsAny<ProductsByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            // Act
            var result = await _handler.Handle(new ViewCartCommand(), CancellationToken.None);

            // Assert
            Assert.Empty(result.Value.Items);
            Assert.True(result.Value.HasDeletedItem);

            // Verify side effect: RemoveItemAsync was called for the missing ID
            _cartServiceMock.Verify(x => x.RemoveItemAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- Helpers ---

        private Cart CreateTestCart(Guid productId, decimal snapshotPrice)
        {
            return new Cart
            {
                LastModifiedDate = DateTime.UtcNow,
                CartItems = new List<CartItem>
                {
                    new()
                    {
                        Id = 10,
                        ProductId = productId,
                        UnitPriceSnapshot = snapshotPrice,
                        Quantity = 1
                    }
                }
            };
        }

        private Product CreateTestProduct(Guid id, decimal price, bool isPublished)
        {
            return new Product
            {
                Id = id,
                Price = price,
                Status = isPublished ? ProductStatus.Published : ProductStatus.Draft,
                IsDeleted = false
            };
        }
    }
}
