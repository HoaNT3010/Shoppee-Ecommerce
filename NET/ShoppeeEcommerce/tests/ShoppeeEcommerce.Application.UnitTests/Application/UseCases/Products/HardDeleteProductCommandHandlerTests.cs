using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.Storage;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.HardDelete;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class HardDeleteProductCommandHandlerTests
    : CommandHandlerTestBase<Product, Guid>
    {
        private readonly Mock<IFileService> _fileServiceMock;

        public HardDeleteProductCommandHandlerTests()
        {
            _fileServiceMock = new Mock<IFileService>();
        }

        private HardDeleteProductCommandHandler CreateHandler(
            Mock<ILogger<HardDeleteProductCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<HardDeleteProductCommandHandler>();
            return new HardDeleteProductCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                logger.Object,
                _fileServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new HardDeleteProductCommand(productId);

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
        public async Task Handle_ShouldDeleteFromRepoAndCloud_WhenProductExists()
        {
            // Arrange
            var product = new Product();
            product.ProductImages.Add(new ProductImage { PublicId = "cloudinary_1", IsMain = true });
            product.ProductImages.Add(new ProductImage { PublicId = "cloudinary_2", IsMain = false });

            var command = new HardDeleteProductCommand(product.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Deleted, result.Value);

            // Verify Repository Delete was called
            RepoMock.Verify(r => r.Delete(product), Times.Once);

            // Verify File Service was called with the correct PublicIds
            _fileServiceMock.Verify(fs => fs.DeleteManyAsync(
                It.Is<IEnumerable<string>>(ids => ids.Contains("cloudinary_1") && ids.Contains("cloudinary_2")),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // Verify SaveChanges
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnHardDeleteFailed_WhenFileServiceThrowsException()
        {
            // Arrange
            var product = new Product();
            product.ProductImages.Add(new ProductImage { PublicId = "id_1" });
            var command = new HardDeleteProductCommand(product.Id);
            var loggerMock = CreateLoggerMock<HardDeleteProductCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Simulate Cloudinary failure
            _fileServiceMock.Setup(fs => fs.DeleteManyAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Cloudinary API Timeout"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.HardDeleteFailed(), result.FirstError);

            // Ensure SaveChanges was NEVER called because the process failed mid-way
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            VerifyErrorLog(loggerMock, Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldReturnHardDeleteFailed_WhenDatabaseSaveFails()
        {
            // Arrange
            var product = new Product();
            var command = new HardDeleteProductCommand(product.Id);
            var loggerMock = CreateLoggerMock<HardDeleteProductCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Foreign key constraint violation"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.HardDeleteFailed(), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
