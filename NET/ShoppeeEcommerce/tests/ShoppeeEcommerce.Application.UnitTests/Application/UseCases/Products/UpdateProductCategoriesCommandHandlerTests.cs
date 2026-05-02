using ErrorOr;
using Microsoft.Extensions.Logging;
using Moq;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.UpdateCategories;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Products
{
    public class UpdateProductCategoriesCommandHandlerTests
    : CommandHandlerTestBase<Product, Guid>
    {
        private readonly Mock<IRepository<Category, Guid>> _cateRepoMock;

        public UpdateProductCategoriesCommandHandlerTests()
        {
            _cateRepoMock = new Mock<IRepository<Category, Guid>>();
        }

        private UpdateProductCategoriesCommandHandler CreateHandler(
            Mock<ILogger<UpdateProductCategoriesCommandHandler>>? loggerMock = null)
        {
            var logger = loggerMock ?? CreateLoggerMock<UpdateProductCategoriesCommandHandler>();
            return new UpdateProductCategoriesCommandHandler(
                RepoMock.Object,
                _cateRepoMock.Object,
                UoWMock.Object,
                logger.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new UpdateProductCategoriesCommand(productId, new List<Guid> { Guid.NewGuid() });

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
        public async Task Handle_ShouldReturnInvalidCategoryIds_WhenSomeCategoriesAreMissingInDb()
        {
            // Arrange
            var product = new Product();
            var categoryIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var command = new UpdateProductCategoriesCommand(product.Id, categoryIds);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Return only 1 category instead of 2
            _cateRepoMock.Setup(r => r.ListAsync(It.IsAny<CategoriesByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category> { new Category() });

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.CategoryErrors.InvalidCategoryIds(), result.FirstError);
        }

        [Fact]
        public async Task Handle_ShouldUpdateCategories_WhenAllIdsAreValid()
        {
            // Arrange
            var product = new Product();
            product.Categories.Add(new Category { Id = Guid.NewGuid(), Name = "Old Category" });

            var newCategory1 = new Category { Id = Guid.NewGuid(), Name = "Tech" };
            var newCategory2 = new Category { Id = Guid.NewGuid(), Name = "Gadgets" };
            var newCategoryIds = new List<Guid> { newCategory1.Id, newCategory2.Id };

            var command = new UpdateProductCategoriesCommand(product.Id, newCategoryIds);

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _cateRepoMock.Setup(r => r.ListAsync(It.IsAny<CategoriesByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category> { newCategory1, newCategory2 });

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(Result.Updated, result.Value);

            // Verify state changes
            Assert.Equal(2, product.Categories.Count);
            Assert.Contains(product.Categories, c => c.Id == newCategory1.Id);
            Assert.Contains(product.Categories, c => c.Id == newCategory2.Id);
            Assert.DoesNotContain(product.Categories, c => c.Name == "Old Category");

            UoWMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnUpdateFailedAndLog_WhenExceptionOccurs()
        {
            // Arrange
            var product = new Product();
            var command = new UpdateProductCategoriesCommand(product.Id, new List<Guid> { Guid.NewGuid() });
            var loggerMock = CreateLoggerMock<UpdateProductCategoriesCommandHandler>();

            RepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ProductByIdSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            _cateRepoMock.Setup(r => r.ListAsync(It.IsAny<CategoriesByIdsSpec>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Category> { new Category() });

            UoWMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database connection lost"));

            var handler = CreateHandler(loggerMock);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Errors.ProductErrors.UpdateFailed(), result.FirstError);
            VerifyErrorLog(loggerMock, Times.Once());
        }
    }
}
