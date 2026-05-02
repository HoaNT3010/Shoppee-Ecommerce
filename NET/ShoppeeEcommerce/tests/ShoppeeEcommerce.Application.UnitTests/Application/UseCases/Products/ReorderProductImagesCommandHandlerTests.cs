using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.ReorderImages;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ReorderImages;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class ReorderProductImagesCommandHandlerTests
    : CommandHandlerTestBase<Product, Guid>
    {
        private ReorderProductImagesCommandHandler CreateHandler(
            Mock<ILogger<ReorderProductImagesCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<ReorderProductImagesCommandHandler>();
            return new ReorderProductImagesCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new ReorderProductImagesCommand(productId, new List<ProductImageItem>());

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.ProductNotFoundWithId(productId.ToString()), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnHasNoImages_WhenProductImageCollectionIsEmpty()
        {
            // Arrange
            var product = new Product(); // No images
            var command = new ReorderProductImagesCommand(product.Id, new List<ProductImageItem>());

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.HasNoImages, result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnImagesNotFound_WhenRequestContainsIdsNotBelongingToProduct()
        {
            // Arrange
            var product = new Product();
            var validImageId = 1;
            product.ProductImages.Add(new ProductImage { Id = validImageId });

            var invalidImageId = 10;
            var orders = new List<ProductImageItem>
            {
                new ProductImageItem { ImageId = validImageId, DisplayOrder = 1 },
                new ProductImageItem { ImageId = invalidImageId, DisplayOrder = 2 }
            };

            var command = new ReorderProductImagesCommand(product.Id, orders);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.ImagesNotFound(new[] { invalidImageId }), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldUpdateDisplayOrders_WhenAllIdsAreValid()
        {
            // Arrange
            var product = new Product();
            var img1 = new ProductImage { Id = 1, DisplayOrder = 0 };
            var img2 = new ProductImage { Id = 2, DisplayOrder = 1 };
            product.ProductImages.Add(img1);
            product.ProductImages.Add(img2);

            var orders = new List<ProductImageItem>
            {
                new ProductImageItem { ImageId = img1.Id, DisplayOrder = 10 },
                new ProductImageItem { ImageId = img2.Id, DisplayOrder = 20 }
            };

            var command = new ReorderProductImagesCommand(product.Id, orders);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify state changes
            Assert.Equal(10, img1.DisplayOrder);
            Assert.Equal(20, img2.DisplayOrder);

            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdateImagesFailedAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var product = new Product();
            var imgId = 1;
            product.ProductImages.Add(new ProductImage { Id = imgId });

            var command = new ReorderProductImagesCommand(product.Id, new List<ProductImageItem> { new ProductImageItem { ImageId = imgId, DisplayOrder = 1 } });
            var loggerMock = CreateLoggerMock<ReorderProductImagesCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database deadlock"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.UpdateImagesFailed(product.Id.ToString()), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
