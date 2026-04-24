using Moq;
using ShoppeeEcommerce.Application.Tests.Application.Common;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Categories.GetAll;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;

namespace ShoppeeEcommerce.Application.Tests.Application.UseCases.Categories
{
    public class GetAllCategoriesQueryHandlerTests
        : QueryHandlerTestBase<Category, Guid>
    {
        GetAllCategoriesQueryHandler CreateHandler()
            => new(Repo);

        [Fact]
        public async Task Handle_ShouldCallRepo_AndReturnResult()
        {
            // Arrange
            var expected = new List<BaseCategoryResponse>();
            RepoMock.Setup(r => r.ListAsync(
                    It.IsAny<GetAllCategoriesSpec>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);
            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(new GetAllCategoriesQuery(), default);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(expected, result.Value);
            RepoMock.Verify(r => r.ListAsync(
                It.IsAny<GetAllCategoriesSpec>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
