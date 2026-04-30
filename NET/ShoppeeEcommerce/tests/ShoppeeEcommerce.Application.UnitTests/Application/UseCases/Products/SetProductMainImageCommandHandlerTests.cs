using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.SetMainImage;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class SetProductMainImageCommandHandlerTests
    : CommandHandlerTestBase<Product, Guid>
    {
        private SetProductMainImageCommandHandler CreateHandler(
            Mock<ILogger<SetProductMainImageCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<SetProductMainImageCommandHandler>();
            return new SetProductMainImageCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new SetProductMainImageCommand(productId, 1);

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
        public async Task Handle_ShouldReturnEmptyProductImages_WhenProductHasNoImages()
        {
            // Arrange
            var product = new Product(); // Empty images list by default
            var command = new SetProductMainImageCommand(product.Id, 1);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.EmptyProductImages(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnHasNoSpecificImage_WhenImageIdIsNotInProductList()
        {
            // Arrange
            var product = new Product();
            product.ProductImages.Add(new ProductImage { Id = 1 });

            var randomImageId = 2;
            var command = new SetProductMainImageCommand(product.Id, randomImageId);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.HasNoSpecificImage(randomImageId, product.Id.ToString()), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdatedImmediately_WhenTargetImageIsAlreadyMain()
        {
            // Arrange
            var imageId = 1;
            var product = new Product();
            product.ProductImages.Add(new ProductImage { Id = imageId, IsMain = true });

            var command = new SetProductMainImageCommand(product.Id, imageId);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify no DB changes were attempted
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldSwapMainImageAndSaveTwice_WhenValid()
        {
            // Arrange
            var oldMainId = 1;
            var newMainId = 2;
            var product = new Product();

            var oldMain = new ProductImage { Id = oldMainId, IsMain = true };
            var newMain = new ProductImage { Id = newMainId, IsMain = false };

            product.ProductImages.Add(oldMain);
            product.ProductImages.Add(newMain);

            var command = new SetProductMainImageCommand(product.Id, newMainId);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify Logic
            Assert.False(oldMain.IsMain);
            Assert.True(newMain.IsMain);

            // Verify double save behavior (once for reset, once for set)
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdateMainImageFailedAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var imageId = 1;
            var product = new Product();
            product.ProductImages.Add(new ProductImage { Id = imageId, IsMain = false });

            var command = new SetProductMainImageCommand(product.Id, imageId);
            var loggerMock = CreateLoggerMock<SetProductMainImageCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Throw exception on first save
            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Concurrency violation"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.UpdateMainImageFailed(product.Id.ToString()), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
