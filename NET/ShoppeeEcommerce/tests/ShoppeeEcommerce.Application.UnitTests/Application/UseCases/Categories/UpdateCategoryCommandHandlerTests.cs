using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Categories.Update;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Categories
{
    public class UpdateCategoryCommandHandlerTests
        : CommandHandlerTestBase<Category, Guid>
    {
        private UpdateCategoryCommandHandler CreateHandler(
            Mock<ILogger<UpdateCategoryCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<UpdateCategoryCommandHandler>();
            return new UpdateCategoryCommandHandler(UoWMock.Object, RepoMock.Object, logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdated_WhenCommandHasNoChanges()
        {
            // Arrange
            var command = new UpdateCategoryCommand(Guid.NewGuid(), string.Empty, null);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(Result.Updated, result.Value);
            RepoMock.Verify(r => r.FirstOrDefaultAsync(It.IsAny<GetCategoryByIdSpec>(), It.IsAny<CancellationToken>()), Times.Never);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new UpdateCategoryCommand(id, "New Name", null);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetCategoryByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.NotFoundWithId(id.ToString()), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnDuplicatedNameError_WhenNewNameAlreadyExists()
        {
            // Arrange
            var category = new Category { Name = "Old Name" };
            var command = new UpdateCategoryCommand(category.Id, "Existing Name", null);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetCategoryByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            RepoMock.Setup(r => r.AnyAsync(It.IsAny<DuplicatedCategoryNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.CategoryNameDuplicated(command.Name!), result.FirstError);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldUpdatePropertiesAndSave_WhenValid()
        {
            // Arrange
            var category = new Category { Name = "Old Name", Description = "Old Desc" };
            var command = new UpdateCategoryCommand(category.Id, "New Name", "New Desc");

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetCategoryByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            RepoMock.Setup(r => r.AnyAsync(It.IsAny<DuplicatedCategoryNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.Equal(Result.Updated, result.Value);
            Assert.Equal("New Name", category.Name);
            Assert.Equal("New Desc", category.Description);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureErrorAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var category = new Category { Name = "Old Name" };
            var command = new UpdateCategoryCommand(category.Id, "New Name", null);
            var loggerMock = CreateLoggerMock<UpdateCategoryCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetCategoryByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);

            RepoMock.Setup(r => r.AnyAsync(It.IsAny<DuplicatedCategoryNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Concurrency error"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.UpdateCategoryFailed(), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
