using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Categories.HardDelete;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Categories
{
    public class HardDeleteCategoryCommandHandlerTests
        : CommandHandlerTestBase<Category, Guid>
    {
        private HardDeleteCategoryCommandHandler CreateHandler(
            Mock<ILogger<HardDeleteCategoryCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<HardDeleteCategoryCommandHandler>();
            return new HardDeleteCategoryCommandHandler(RepoMock.Object, UoWMock.Object, logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new HardDeleteCategoryCommand(id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.NotFoundWithId(id.ToString()), result.FirstError);
            RepoMock.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldHardDeleteAndSave_WhenCategoryExists()
        {
            // Arrange
            var category = new Category { Id = Guid.NewGuid() };
            var command = new HardDeleteCategoryCommand(category.Id);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Deleted, result.Value);
            RepoMock.Verify(r => r.Delete(category), Times.Once);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureErrorAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var category = new Category { Id = Guid.NewGuid() };
            var command = new HardDeleteCategoryCommand(category.Id);
            var loggerMock = CreateLoggerMock<HardDeleteCategoryCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database constraint violation"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.HardDeleteCategoryFailed(), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
