using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications
{
    internal class ProductsByIdsSpec
        : Specification<Product>
    {
        public ProductsByIdsSpec(
            IEnumerable<Guid> Ids,
            bool asTracking = false,
            bool ignoreQueryFilter = false)
        {
            Query.Where(p => Ids.Contains(p.Id))
                .Include(p => p.ProductImages.Where(i => i.IsMain))
                .AsTracking(asTracking)
                .IgnoreQueryFilters(ignoreQueryFilter);
        }
    }
}
