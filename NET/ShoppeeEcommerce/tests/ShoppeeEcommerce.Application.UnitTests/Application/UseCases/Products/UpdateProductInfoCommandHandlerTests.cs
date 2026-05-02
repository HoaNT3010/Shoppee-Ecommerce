using Ardalis.Specification;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.UpdateInfo;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class UpdateProductInfoCommandHandlerTests
    : CommandHandlerTestBase<Product, Guid>
    {
        private UpdateProductInfoCommandHandler CreateHandler(
            Mock<ILogger<UpdateProductInfoCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<UpdateProductInfoCommandHandler>();
            return new UpdateProductInfoCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdatedImmediately_WhenNoChangesProvided()
        {
            // Arrange
            // All updateable fields are null/empty
            var command = new UpdateProductInfoCommand(Guid.NewGuid(), null, null, null, null);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify repository was never even touched
            RepoMock.Verify(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Product>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new UpdateProductInfoCommand(productId, "New Name", null, null, null);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.ProductNotFoundWithId(productId.ToString()), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnProductNameExisted_WhenNewNameIsDuplicated()
        {
            // Arrange
            var product = new Product { Name = "Old Name" };
            var newName = "Existing Name";
            var command = new UpdateProductInfoCommand(product.Id, newName, null, null, null);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Mock that the name already exists in another product
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.ProductNameExisted(newName), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnProductSKUExisted_WhenNewSKUIsDuplicated()
        {
            // Arrange
            var product = new Product { SKU = "OLD-SKU" };
            var newSku = "DUPE-SKU";
            var command = new UpdateProductInfoCommand(product.Id, null, null, null, newSku);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Mock that the SKU already exists
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithSKUSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.ProductSKUExisted(newSku), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldUpdateAllFields_WhenValidDataProvided()
        {
            // Arrange
            var product = new Product
            {
                Name = "Old Name",
                Description = "Old Desc",
                Price = 10,
                SKU = "OLD-SKU"
            };

            var command = new UpdateProductInfoCommand(
                product.Id,
                "New Name",
                "New Description",
                20.5m,
                "NEW-SKU");

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Mock that Name and SKU are NOT duplicated
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ISpecification<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            Assert.Equal("New Name", product.Name);
            Assert.Equal("New Description", product.Description);
            Assert.Equal(20.5m, product.Price);
            Assert.Equal("NEW-SKU", product.SKU);

            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdateFailedAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var product = new Product { Name = "Target" };
            var command = new UpdateProductInfoCommand(product.Id, "Updated Name", null, null, null);
            var loggerMock = CreateLoggerMock<UpdateProductInfoCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB Update Error"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.UpdateFailed(), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
