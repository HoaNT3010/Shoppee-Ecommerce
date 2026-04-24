using ErrorOr;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Categories.SoftDelete;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Categories
{
    public class SoftDeleteCategoryCommandHandlerTests
        : CommandHandlerTestBase<Category, Guid>
    {
        SoftDeleteCategoryCommandHandler CreateHandler()
        {
            var logger = CreateLoggerMock<SoftDeleteCategoryCommandHandler>();
            return new SoftDeleteCategoryCommandHandler(RepoMock.Object, UoWMock.Object, logger.Object);
        }

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
            var command = new SoftDeleteCategoryCommand(id);

            // Act
            ErrorOr<Deleted> result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.NotFoundWithId(id.ToString()), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnDeleted_WhenCategoryAlreadyDeleted()
        {
            // Arrange
            var category = new Category
            {
                IsDeleted = true
            };
            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new SoftDeleteCategoryCommand(category.Id), default);

            // Assert
            Assert.Equal(Result.Deleted, result.Value);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldSoftDeleteAndSave_WhenCategoryExists()
        {
            // Arrange
            var category = new Category();
            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new SoftDeleteCategoryCommand(category.Id), default);

            // Assert
            Assert.True(category.IsDeleted);
            Assert.Equal(Result.Deleted, result.Value);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldLogAndReturnError_WhenSaveChangesFails()
        {
            // Arrange
            var loggerMock = CreateLoggerMock<SoftDeleteCategoryCommandHandler>();
            var category = new Category();
            RepoMock.Setup(r => r.FirstOrDefaultAsync(
                    It.IsAny<GetCategoryByIdSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(category);
            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB failure"));
            var handler = new SoftDeleteCategoryCommandHandler(
                RepoMock.Object,
                UoWMock.Object,
                loggerMock.Object);

            // Act
            var result = await handler.Handle(
                new SoftDeleteCategoryCommand(category.Id), default);

            // Assert
            Assert.True(result.IsError);
            VerifyErrorLog(loggerMock, Times.Once());
            Assert.Equal(Errors.CategoryErrors.SoftDeleteCategoryFailed(), result.FirstError);
        }
    }
}
