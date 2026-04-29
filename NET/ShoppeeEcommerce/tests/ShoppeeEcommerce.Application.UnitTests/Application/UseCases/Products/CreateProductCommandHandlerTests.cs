using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.Create;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class CreateProductCommandHandlerTests
        : CommandHandlerTestBase<Product, Guid>
    {
        private readonly Mock<IRepository<Category, Guid>> _categoryRepoMock = new();

        private CreateProductCommandHandler CreateHandler(
            Mock<ILogger<CreateProductCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<CreateProductCommandHandler>();
            return new CreateProductCommandHandler(
                RepoMock.Object,
                _categoryRepoMock.Object,
                UoWMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProductNameExisted_WhenNameAlreadyExists()
        {
            // Arrange
            var command = CreateCommand();
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.ProductNameExisted(command.Name), result.FirstError);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnProductSKUExisted_WhenSKUAlreadyExists()
        {
            // Arrange
            var command = CreateCommand();
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithNameSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithSKUSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.ProductSKUExisted(command.SKU), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldReturnInvalidCategoryIds_WhenSomeCategoriesMissing()
        {
            // Arrange
            var command = CreateCommand();
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithNameSpec>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithSKUSpec>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Return only 1 category when 2 are requested
            _categoryRepoMock.Setup(r => r.ListAsync(It.IsAny<CategoriesByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category> { new() { Name = "Tech" } });

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.InvalidCategoryIds(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldCreateProductAndReturnResponse_WhenValid()
        {
            // Arrange
            var command = CreateCommand();
            var categories = command.CategoryIds.Select(id => new Category { Id = id, Name = "Category " + id }).ToList();

            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithNameSpec>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithSKUSpec>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _categoryRepoMock.Setup(r => r.ListAsync(It.IsAny<CategoriesByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categories);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(command.Name, result.Value.Name);
            Assert.Equal(categories.Count, result.Value.Categories.Count);

            RepoMock.Verify(r => r.AddAsync(It.Is<Product>(p => p.SKU == command.SKU), It.IsAny<CancellationToken>()), Times.Once);
            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureErrorAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var command = CreateCommand();
            var loggerMock = CreateLoggerMock<CreateProductCommandHandler>();

            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithNameSpec>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            RepoMock.Setup(r => r.AnyAsync(It.IsAny<ProductWithSKUSpec>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _categoryRepoMock.Setup(r => r.ListAsync(It.IsAny<CategoriesByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category> { new(), new() });

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB Error"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.CreateProductFailed(), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }

        private static CreateProductCommand CreateCommand() =>
            new("Gaming Laptop", "High performance", 1500m, "LAP-001", [Guid.NewGuid(), Guid.NewGuid()], Guid.NewGuid());
    }
}
