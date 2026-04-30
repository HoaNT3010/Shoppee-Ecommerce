using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Domain.Exceptions;
using ShoppeeEcommerce.Infrastructure.Storage.CloudinaryFS;
using System.Text;

namespace ShoppeeEcommerce.Infrastructure.Tests.Storage.CloudinaryFS
{
    public class CloudinaryFileServiceTests
    {
        private readonly Mock<ICloudinary> _cloudinaryMock;
        private readonly Mock<ILogger<CloudinaryFileService>> _loggerMock;

        public CloudinaryFileServiceTests()
        {
            _cloudinaryMock = new Mock<ICloudinary>();
            _loggerMock = new Mock<ILogger<CloudinaryFileService>>();
        }

        private CloudinaryFileService CreateService()
        {
            // Injecting the mock as the concrete Cloudinary type works because
            // the Cloudinary SDK class implements ICloudinary.
            return new CloudinaryFileService(_cloudinaryMock.Object, _loggerMock.Object);
        }

        #region Upload Tests

        [Fact]
        public async Task UploadAsync_ShouldApplyTransformationSettings_WhenCalled()
        {
            // Arrange
            using var stream = new MemoryStream();
            _cloudinaryMock.Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ImageUploadResult { SecureUrl = new Uri("http://test.com"), StatusCode = System.Net.HttpStatusCode.OK });

            var service = CreateService();

            // Act
            await service.UploadAsync(stream, "test.jpg", "folder", default);

            // Assert
            _cloudinaryMock.Verify(c => c.UploadAsync(
                It.Is<ImageUploadParams>(p =>
                    p.Transformation.ToString().Contains("q_auto") &&
                    p.Transformation.ToString().Contains("f_auto")),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UploadAsync_ShouldReturnFileUploadResult_WhenUploadIsSuccessful()
        {
            // Arrange
            var fileName = "product-image.png";
            var folder = "products";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("fake-file-content"));

            var uploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri("https://res.cloudinary.com/demo/image/upload/v1/sample.png"),
                PublicId = "sample_id",
                Format = "png",
                Bytes = 2048,
                StatusCode = System.Net.HttpStatusCode.OK
            };

            _cloudinaryMock.Setup(c => c.UploadAsync(
                    It.IsAny<ImageUploadParams>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadResult);

            var service = CreateService();

            // Act
            var result = await service.UploadAsync(stream, fileName, folder, default);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(uploadResult.SecureUrl.ToString(), result.Url);
            Assert.Equal(uploadResult.PublicId, result.PublicId);
            Assert.Equal(uploadResult.Format, result.Format);
            Assert.Equal(uploadResult.Bytes, result.Size);

            _cloudinaryMock.Verify(c => c.UploadAsync(
                It.Is<ImageUploadParams>(p =>
                    p.Folder == folder &&
                    p.File.FileName == fileName &&
                    p.UniqueFilename == true),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UploadAsync_ShouldThrowFileUploadException_WhenCloudinaryReturnsError()
        {
            // Arrange
            var fileName = "bad-file.jpg";
            using var stream = new MemoryStream();

            var uploadResult = new ImageUploadResult
            {
                Error = new CloudinaryDotNet.Actions.Error { Message = "Upload preset not found" }
            };

            _cloudinaryMock.Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadResult);

            var service = CreateService();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FileUploadException>(() =>
                service.UploadAsync(stream, fileName, "test-folder", default));

            Assert.Contains("Upload preset not found", exception.Message);

            // Verify Error Logging
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cloudinary upload failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task UploadManyAsync_ShouldReturnEmptyList_WhenNoFilesProvided()
        {
            // Arrange
            var files = Enumerable.Empty<(Stream Stream, string FileName)>();
            var service = CreateService();

            // Act
            var results = await service.UploadManyAsync(files, "folder", default);

            // Assert
            Assert.Empty(results);
            _cloudinaryMock.Verify(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UploadManyAsync_ShouldThrowException_WhenOneUploadFails()
        {
            // Arrange
            var files = new List<(Stream Stream, string FileName)>
        {
            (new MemoryStream(), "success.jpg"),
            (new MemoryStream(), "fail.jpg")
        };

            // First call succeeds, second call returns error
            _cloudinaryMock.SetupSequence(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ImageUploadResult { SecureUrl = new Uri("http://test.com"), StatusCode = System.Net.HttpStatusCode.OK })
                .ReturnsAsync(new ImageUploadResult { Error = new CloudinaryDotNet.Actions.Error { Message = "API Limit" } });

            var service = CreateService();

            // Act & Assert
            // Task.WhenAll will rethrow the first exception encountered
            await Assert.ThrowsAsync<FileUploadException>(() =>
                service.UploadManyAsync(files, "folder", default));

            // Cleanup
            foreach (var file in files) await file.Stream.DisposeAsync();
        }

        [Fact]
        public async Task UploadManyAsync_ShouldUploadAllFiles_WhenAllUploadsSucceed()
        {
            // Arrange
            var folder = "products/batch-1";
            var files = new List<(Stream Stream, string FileName)>
        {
            (new MemoryStream(Encoding.UTF8.GetBytes("file1")), "image1.jpg"),
            (new MemoryStream(Encoding.UTF8.GetBytes("file2")), "image2.jpg"),
            (new MemoryStream(Encoding.UTF8.GetBytes("file3")), "image3.jpg")
        };

            var uploadResult = new ImageUploadResult
            {
                SecureUrl = new Uri("https://res.cloudinary.com/demo/image/upload/v1/sample.png"),
                PublicId = "sample_id",
                Format = "png",
                Bytes = 1024,
                StatusCode = System.Net.HttpStatusCode.OK
            };

            _cloudinaryMock.Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(uploadResult);

            var service = CreateService();

            // Act
            var results = await service.UploadManyAsync(files, folder, default);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(files.Count, results.Count);

            // Verify ICloudinary was called for each file
            _cloudinaryMock.Verify(c => c.UploadAsync(
                It.Is<ImageUploadParams>(p => p.Folder == folder),
                It.IsAny<CancellationToken>()),
                Times.Exactly(files.Count));

            // Cleanup
            foreach (var file in files) await file.Stream.DisposeAsync();
        }
        #endregion Upload Tests

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_ShouldComplete_WhenSuccessful()
        {
            // Arrange
            var publicId = "test_id";
            _cloudinaryMock.Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
                .ReturnsAsync(new DeletionResult { StatusCode = System.Net.HttpStatusCode.OK });

            var service = CreateService();

            // Act
            await service.DeleteAsync(publicId);

            // Assert
            _cloudinaryMock.Verify(c => c.DestroyAsync(It.Is<DeletionParams>(p => p.PublicId == publicId)), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldPropagateException_WhenCloudinaryCommunicationFails()
        {
            // Arrange
            var publicId = "test-id";
            _cloudinaryMock.Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
                .ThrowsAsync(new Exception("Network failure"));

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                service.DeleteAsync(publicId, default));
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowFileUploadException_WhenCloudinaryReturnsError()
        {
            // Arrange
            var publicId = "test_id";
            var deleteResult = new DeletionResult
            {
                Error = new CloudinaryDotNet.Actions.Error { Message = "Not Found" }
            };

            _cloudinaryMock.Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
                .ReturnsAsync(deleteResult);

            var service = CreateService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<FileUploadException>(() => service.DeleteAsync(publicId));
            Assert.Contains("Not Found", ex.Message);
        }

        [Fact]
        public async Task DeleteManyAsync_ShouldHandleParallelExecution()
        {
            // Arrange
            var publicIds = Enumerable.Range(1, 5).Select(i => $"id-{i}").ToList();

            _cloudinaryMock.Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
                .ReturnsAsync(new DeletionResult { StatusCode = System.Net.HttpStatusCode.OK });

            var service = CreateService();

            // Act
            await service.DeleteManyAsync(publicIds, default);

            // Assert
            // Ensure all tasks were awaited and completed
            _cloudinaryMock.Verify(c => c.DestroyAsync(It.IsAny<DeletionParams>()),
                Times.Exactly(5));
        }

        [Fact]
        public async Task DeleteManyAsync_ShouldInvokeDeleteForEachId()
        {
            // Arrange
            var ids = new List<string> { "id1", "id2", "id3" };
            _cloudinaryMock.Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
                .ReturnsAsync(new DeletionResult { StatusCode = System.Net.HttpStatusCode.OK });

            var service = CreateService();

            // Act
            await service.DeleteManyAsync(ids);

            // Assert
            _cloudinaryMock.Verify(c => c.DestroyAsync(It.IsAny<DeletionParams>()), Times.Exactly(3));
        }

        [Fact]
        public async Task DeleteManyAsync_ShouldNotInvokeCloudinary_WhenCollectionIsEmpty()
        {
            // Arrange
            var publicIds = Enumerable.Empty<string>();
            var service = CreateService();

            // Act
            await service.DeleteManyAsync(publicIds, default);

            // Assert
            _cloudinaryMock.Verify(c => c.DestroyAsync(It.IsAny<DeletionParams>()), Times.Never);
        }

        [Fact]
        public async Task DeleteManyAsync_ShouldThrowException_WhenAtLeastOneDeleteFails()
        {
            // Arrange
            var publicIds = new List<string> { "valid-id", "invalid-id" };

            // Setup sequence: first succeeds, second fails
            _cloudinaryMock.SetupSequence(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
                .ReturnsAsync(new DeletionResult { StatusCode = System.Net.HttpStatusCode.OK })
                .ReturnsAsync(new DeletionResult
                {
                    Error = new CloudinaryDotNet.Actions.Error { Message = "Delete failed" }
                });

            var service = CreateService();

            // Act & Assert
            // Task.WhenAll will propagate the exception from the failed task
            var exception = await Assert.ThrowsAsync<FileUploadException>(() =>
                service.DeleteManyAsync(publicIds, default));

            Assert.Contains("Delete failed", exception.Message);
        }
        #endregion Delete Tests
    }
}