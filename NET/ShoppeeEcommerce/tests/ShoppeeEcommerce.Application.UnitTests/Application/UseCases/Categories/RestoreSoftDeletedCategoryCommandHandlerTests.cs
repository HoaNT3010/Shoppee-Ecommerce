using ErrorOr;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Categories.RestoreSoftDeleted;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Categories
{
    public class RestoreSoftDeletedCategoryCommandHandlerTests
        : CommandHandlerTestBase<Category, Guid>
    {
        RestoreSoftDeletedCategoryCommandHandler CreateHandler()
            => new RestoreSoftDeletedCategoryCommandHandler(
                CreateLoggerMock<RestoreSoftDeletedCategoryCommandHandler>().Object,
                RepoMock.Object,
                UoWMock.Object);

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenCategoryNotExist()
        {
            // Arrange
            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);
            var handler = CreateHandler();
            var id = Guid.NewGuid();
            var command = new RestoreSoftDeletedCategoryCommand(id);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.NotFoundWithId(id.ToString()), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdated_WhenCategoryAlreadyRestored()
        {
            // Arrange
            var category = new Category
            {
                IsDeleted = false
            };
            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new RestoreSoftDeletedCategoryCommand(category.Id), default);

            // Assert
            Assert.Equal(Result.Updated, result.Value);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldRestoreSoftDeletedAndSave_WhenCategoryExists()
        {
            // Arrange
            var category = new Category() { IsDeleted = true };
            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new RestoreSoftDeletedCategoryCommand(category.Id), default);

            // Assert
            Assert.False(category.IsDeleted);
            Assert.Equal(Result.Updated, result.Value);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldLogAndReturnError_WhenSaveChangesFails()
        {
            // Arrange
            var loggerMock = CreateLoggerMock<RestoreSoftDeletedCategoryCommandHandler>();
            var category = new Category() { IsDeleted = true };
            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB failure"));
            var handler = new RestoreSoftDeletedCategoryCommandHandler(
                loggerMock.Object,
                RepoMock.Object,
                UoWMock.Object);

            // Act
            var result = await handler.Handle(
                new RestoreSoftDeletedCategoryCommand(category.Id), default);

            // Assert
            Assert.True(result.IsError);
            VerifyErrorLog(loggerMock, Times.Once());
            Assert.Equal(Errors.CategoryErrors.RestoreSoftDeletedCategoryFailed(), result.FirstError);
        }
    }
}
