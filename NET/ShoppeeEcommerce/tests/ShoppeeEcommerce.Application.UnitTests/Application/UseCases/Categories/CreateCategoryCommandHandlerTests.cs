using ErrorOr;
using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Categories.Create;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Categories
{
    public class CreateCategoryCommandHandlerTests
        : CommandHandlerTestBase<Category, Guid>
    {
        private CreateCategoryCommandHandler CreateHandler()
        {
            var logger = CreateLoggerMock<CreateCategoryCommandHandler>();
            return new CreateCategoryCommandHandler(RepoMock.Object, logger.Object, UoWMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDuplicatedNameError_WhenCategoryNameAlreadyExists()
        {
            // Arrange
            var command = new CreateCategoryCommand("Electronics", "Description", Guid.NewGuid());

            RepoMock.Setup(r => r.AnyAsync(
                    It.IsAny<DuplicatedCategoryNameSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.CategoryNameDuplicated(command.Name), result.FirstError);
            RepoMock.Verify(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnCreated_WhenCategoryIsUniqueAndSaved()
        {
            // Arrange
            var command = new CreateCategoryCommand("Books", "Reading material", Guid.NewGuid());

            RepoMock.Setup(r => r.AnyAsync(
                    It.IsAny<DuplicatedCategoryNameSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Created, result.Value);

            RepoMock.Verify(r => r.AddAsync(
                It.Is<Category>(c => c.Name == command.Name && c.CreatorId == command.CreatorId),
                It.IsAny<CancellationToken>()),
                Times.Once);

            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureErrorAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var command = new CreateCategoryCommand("Hardware", "Tools", Guid.NewGuid());
            var loggerMock = CreateLoggerMock<CreateCategoryCommandHandler>();

            RepoMock.Setup(r => r.AnyAsync(
                    It.IsAny<DuplicatedCategoryNameSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Injecting the specific logger mock to verify it later
            var handler = new CreateCategoryCommandHandler(RepoMock.Object, loggerMock.Object, UoWMock.Object);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.CreateCategoryFailed(), result.FirstError);

            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
