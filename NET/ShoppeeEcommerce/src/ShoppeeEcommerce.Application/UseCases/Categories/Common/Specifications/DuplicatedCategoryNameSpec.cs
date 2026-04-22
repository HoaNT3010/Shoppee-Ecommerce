using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications
{
    internal class DuplicatedCategoryNameSpec
        : Specification<Category>
    {
        public DuplicatedCategoryNameSpec(string categoryName, bool ignoreQueryFilter = true)
        {
            Query.Where(c => c.Name == categoryName)
                .AsNoTracking()
                .IgnoreQueryFilters(ignoreQueryFilter);
        }
    }
}
