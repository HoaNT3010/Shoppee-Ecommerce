using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.RestoreSoftDeleted;
using ShoppeeEcommerce.Application.UseCases.Products.SoftDelete;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class RestoreSoftDeletedProductCommandHandlerTests
    : CommandHandlerTestBase<Product, Guid>
    {
        private RestoreSoftDeletedProductCommandHandler CreateHandler(
            Mock<ILogger<SoftDeleteProductCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<SoftDeleteProductCommandHandler>();
            return new RestoreSoftDeletedProductCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new RestoreSoftDeletedProductCommand(productId);

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
        public async Task Handle_ShouldReturnUpdatedImmediately_WhenProductIsNotDeleted()
        {
            // Arrange
            var product = new Product { IsDeleted = false };
            var command = new RestoreSoftDeletedProductCommand(product.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify no DB save was attempted because product is already active
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldRestoreAndSave_WhenProductIsDeleted()
        {
            // Arrange
            var product = new Product { IsDeleted = true };
            var command = new RestoreSoftDeletedProductCommand(product.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify domain logic state change
            Assert.False(product.IsDeleted);

            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnRestoreFailedAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var product = new Product { IsDeleted = true };
            var command = new RestoreSoftDeletedProductCommand(product.Id);
            var loggerMock = CreateLoggerMock<SoftDeleteProductCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection failure"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.RestoreSoftDeletedFailed(), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
