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
            bool ignoreQueryFilter = false)
        {
            Query.Where(x => x.Id == id)
                .AsTracking(asTracking)
                .IgnoreQueryFilters(ignoreQueryFilter);
            if (includeImages)
                Query.Include(x => x.ProductImages);
        }
    }
}
