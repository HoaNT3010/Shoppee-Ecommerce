using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.Feature;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class FeatureProductCommandHandlerTests
    : CommandHandlerTestBase<Product, Guid>
    {
        private FeatureProductCommandHandler CreateHandler(
            Mock<ILogger<FeatureProductCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<FeatureProductCommandHandler>();
            return new FeatureProductCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new FeatureProductCommand(productId);

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
        public async Task Handle_ShouldReturnUpdatedImmediately_WhenProductIsAlreadyFeatured()
        {
            // Arrange
            var product = new Product { IsFeatured = true };
            var command = new FeatureProductCommand(product.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify no DB save was attempted because no change is needed
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldSetIsFeaturedToTrueAndSave_WhenProductIsNotFeatured()
        {
            // Arrange
            var product = new Product { IsFeatured = false };
            var command = new FeatureProductCommand(product.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify state change
            Assert.True(product.IsFeatured);

            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFeatureFailedAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var product = new Product { IsFeatured = false };
            var command = new FeatureProductCommand(product.Id);
            var loggerMock = CreateLoggerMock<FeatureProductCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Concurrency exception"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.FeatureFailed(), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
