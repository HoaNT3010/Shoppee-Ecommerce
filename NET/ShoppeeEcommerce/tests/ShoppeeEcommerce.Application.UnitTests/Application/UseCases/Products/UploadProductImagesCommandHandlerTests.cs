using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.Storage;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.UploadImages;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class UploadProductImagesCommandHandlerTests
        : CommandHandlerTestBase<Product, Guid>
    {
        private readonly Mock<IFileService> _fileServiceMock = new();

        private UploadProductImagesCommandHandler CreateHandler(
            Mock<ILogger<UploadProductImagesCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<UploadProductImagesCommandHandler>();
            return new UploadProductImagesCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                _fileServiceMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new UploadProductImagesCommand(new FormFileCollection(), productId);

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
        public async Task Handle_ShouldReturnExceedMaximumImages_WhenImageCountExceedsLimit()
        {
            // Arrange
            var product = new Product();
            // Fill product with 9 existing images
            for (int i = 0; i < 9; i++) product.ProductImages.Add(new ProductImage());

            // Try to upload 2 more (Total 11 > 10)
            var files = CreateMockFiles(2);
            var command = new UploadProductImagesCommand(files, product.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.ExceedMaximumImages(10), result.FirstError);
            _fileServiceMock.Verify(fs => fs.UploadManyAsync(It.IsAny<IEnumerable<(Stream, string)>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUploadImagesAndSave_WhenValid()
        {
            // Arrange
            var product = new Product { Name = "Test Product" };
            var files = CreateMockFiles(2);
            var command = new UploadProductImagesCommand(files, product.Id);

            var uploadResults = new List<FileUploadResult>
            {
                new("url1", "public1", "format1", 1 * 1024 * 1024),
                new("url2", "public2", "format2", 1 * 1024 * 1024)
            };

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _fileServiceMock.Setup(fs => fs.UploadManyAsync(
                    It.IsAny<IEnumerable<(Stream, string)>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadResults);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(2, result.Value.Images.Count);
            Assert.Equal(2, product.ProductImages.Count);
            Assert.Equal("url1", product.ProductImages[0].Url);
            Assert.Equal(1, product.ProductImages[0].DisplayOrder); // First image order

            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureErrorAndLog_WhenFileServiceThrows()
        {
            // Arrange
            var product = new Product();
            var files = CreateMockFiles(1);
            var command = new UploadProductImagesCommand(files, product.Id);
            var loggerMock = CreateLoggerMock<UploadProductImagesCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _fileServiceMock.Setup(fs => fs.UploadManyAsync(It.IsAny<IEnumerable<(Stream, string)>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Storage full"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.UpdateImagesFailed(product.Id.ToString()), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }

        private IFormFileCollection CreateMockFiles(int count)
        {
            var collection = new FormFileCollection();
            for (int i = 0; i < count; i++)
            {
                var fileMock = new Mock<IFormFile>();
                fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
                fileMock.Setup(f => f.FileName).Returns($"image{i}.jpg");
                collection.Add(fileMock.Object);
            }
            return collection;
        }
    }
}
