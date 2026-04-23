using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications
{
    internal class GetCategoryByIdSpec
        : Specification<Category>
    {
        public GetCategoryByIdSpec(
            Guid id,
            bool asTracking = true,
            bool ignoreQueryFilter = true)
        {
            Query.Where(c => c.Id == id)
                .AsTracking(asTracking)
                // Usually, query filter is disable when trying to get category by id
                // Administrator operations often use this spec
                // and they also need to query soft-deleted category.
                .IgnoreQueryFilters(asTracking);
        }
    }
}
