using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.UnFeature;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class UnFeatureProductCommandHandlerTests
    : CommandHandlerTestBase<Product, Guid>
    {
        private UnFeatureProductCommandHandler CreateHandler(
            Mock<ILogger<UnFeatureProductCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<UnFeatureProductCommandHandler>();
            return new UnFeatureProductCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new UnFeatureProductCommand(productId);

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
        public async Task Handle_ShouldReturnUpdatedImmediately_WhenProductIsAlreadyNotFeatured()
        {
            // Arrange
            var product = new Product { IsFeatured = false };
            var command = new UnFeatureProductCommand(product.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify no DB save was attempted because the state already matches the request
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldSetIsFeaturedToFalseAndSave_WhenProductIsCurrentlyFeatured()
        {
            // Arrange
            var product = new Product { IsFeatured = true };
            var command = new UnFeatureProductCommand(product.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify state change
            Assert.False(product.IsFeatured);

            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnFeatureFailedAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var product = new Product { IsFeatured = true };
            var command = new UnFeatureProductCommand(product.Id);
            var loggerMock = CreateLoggerMock<UnFeatureProductCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetBaseProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database timeout"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.UnFeatureFailed(), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
