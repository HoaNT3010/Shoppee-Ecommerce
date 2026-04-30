using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications
{
    internal class ProductByIdSpec
        : Specification<Product>
    {
        public ProductByIdSpec(Guid id,
            bool includeImages = true,
            bool asTracking = false,
            bool ignoreQueryFilter = false,
            bool includeCategories = false,
            bool includeCreator = false,
            bool onlyActiveProduct = true,
            bool asSplitQuery = true)
        {
            Query.Where(x => x.Id == id)
                .AsTracking(asTracking)
                .IgnoreQueryFilters(ignoreQueryFilter)
                .AsSplitQuery(asSplitQuery);
            if (includeImages)
                Query.Include(x => x.ProductImages);
            if (includeCategories)
                Query.Include(x => x.Categories);
            if (includeCreator)
                Query.Include(x => x.Creator);
            if (onlyActiveProduct)
                Query.Where(x => x.Status == Domain.Enums.ProductStatus.Published);
        }
    }
}
