using Ardalis.Specification;
using ShoppeeEcommerce.Application.Common.Query;
using ShoppeeEcommerce.Application.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Application.Tests.CommonTests
{
    public class CommonSpecificationExtensionsTests
    {
        public class TestEntity : SoftDeletableEntity<Guid>
        {
            public string Name { get; set; } = string.Empty;
        }

        public class TestSpec : Specification<TestEntity>
        {
            public ISpecificationBuilder<TestEntity> Builder => Query;
        }

        [Fact]
        public void ApplyPaging_ShouldSetSkipAndTakeCorrecty()
        {
            // Arrange
            var spec = new TestSpec();
            int page = 2;
            int pageSize = 15;

            // Act
            spec.Builder.ApplyPaging(page, pageSize);

            // Assert
            Assert.Equal(15, spec.Take);
            Assert.Equal(15, spec.Skip);
        }

        [Fact]
        public void ApplySorting_ShouldDefaultToCreatedDateDescending_WhenSortByIsNull()
        {
            // Arrange
            var spec = new TestSpec();

            // Act
            spec.Builder.ApplySorting(null, null);

            // Assert
            var orderExpression = spec.OrderExpressions.First();
            Assert.Equal(OrderTypeEnum.OrderByDescending, orderExpression.OrderType);
        }

        [Fact]
        public void ApplySorting_ShouldSortBySpecifiedProperty()
        {
            // Arrange
            var spec = new TestSpec();
            string sortBy = "Name";

            // Act
            spec.Builder.ApplySorting(sortBy, sortDesc: false);

            // Assert
            var orderExpression = spec.OrderExpressions.First();
            Assert.Equal(OrderTypeEnum.OrderBy, orderExpression.OrderType);
            Assert.Contains("Name", orderExpression.KeySelector.ToString());
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData(null, false)]
        public void ApplyCommonFilters_ShouldHandleIgnoreQueryFiltersBasedOnIncludeDeleted(
            bool? includeDeleted, bool expectedIgnoreFilters)
        {
            // Arrange
            var spec = new TestSpec();
            var query = new DateRangesSortedPagedIncludeDeletedQuery { IncludeDeleted = includeDeleted };

            // Act
            spec.Builder.ApplyCommonFilters(query);

            // Assert
            Assert.Equal(expectedIgnoreFilters, spec.IgnoreQueryFilters);
        }

        [Fact]
        public void ApplySorting_ShouldThrow_WhenPropertyDoesNotExist()
        {
            // Arrange
            var spec = new TestSpec();

            // Act
            var act = () => spec.Builder.ApplySorting("InvalidName", false);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }
    }
}
