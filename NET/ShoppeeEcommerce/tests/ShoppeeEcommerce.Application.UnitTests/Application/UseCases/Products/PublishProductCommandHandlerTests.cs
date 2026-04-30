using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.PublishProduct;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class PublishProductCommandHandlerTests
        : CommandHandlerTestBase<Product, Guid>
    {
        private PublishProductCommandHandler CreateHandler(
            Mock<ILogger<PublishProductCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<PublishProductCommandHandler>();
            return new PublishProductCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new PublishProductCommand(productId);

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
        public async Task Handle_ShouldReturnAlreadyPublished_WhenStatusIsAlreadyPublished()
        {
            // Arrange
            var product = new Product { Status = ProductStatus.Published };
            var command = new PublishProductCommand(product.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.AlreadyPublished(product.Id.ToString()), result.FirstError);
        }

        [Theory]
        [InlineData(0, 1, true, "InvalidPrice")]      // Invalid Price
        [InlineData(100, 0, true, "MissingCategories")] // Missing Categories
        [InlineData(100, 1, false, "MissingImages")]   // Missing Images
        public async Task Handle_ShouldReturnValidationError_WhenProductIsIncomplete(
            decimal price, int categoryCount, bool hasImages, string expectedErrorKey)
        {
            // Arrange
            var product = new Product { Status = ProductStatus.Draft, Price = price };

            for (int i = 0; i < categoryCount; i++) product.Categories.Add(new Category());
            if (hasImages) product.ProductImages.Add(new ProductImage { IsMain = true });

            var command = new PublishProductCommand(product.Id);
            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            // Checking if the error list contains the specific validation error
            Assert.Contains(result.Errors, e => e.Code.Contains(expectedErrorKey));
        }

        [Fact]
        public async Task Handle_ShouldReturnMissingMainImage_WhenImagesExistButNoneAreMain()
        {
            // Arrange
            var product = new Product
            {
                Status = ProductStatus.Draft,
                Price = 10.0m
            };
            product.Categories.Add(new Category());
            product.ProductImages.Add(new ProductImage { IsMain = false }); // No main image

            var command = new PublishProductCommand(product.Id);
            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.MissingMainImage, result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldPublishProduct_WhenValid()
        {
            // Arrange
            var product = new Product
            {
                Status = ProductStatus.Draft,
                Price = 50.0m
            };
            product.Categories.Add(new Category());
            product.ProductImages.Add(new ProductImage { IsMain = true });

            var command = new PublishProductCommand(product.Id);
            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal(ProductStatus.Published, product.Status);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnPublishFailed_WhenExceptionOccurs()
        {
            // Arrange
            var product = new Product { Price = 100, Status = ProductStatus.Draft };
            product.Categories.Add(new Category());
            product.ProductImages.Add(new ProductImage { IsMain = true });

            var command = new PublishProductCommand(product.Id);
            var loggerMock = CreateLoggerMock<PublishProductCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database offline"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.PublishProductFailed(product.Id.ToString()), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
