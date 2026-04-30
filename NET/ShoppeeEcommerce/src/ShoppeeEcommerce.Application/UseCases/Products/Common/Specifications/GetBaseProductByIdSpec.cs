using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications
{
    internal class GetBaseProductByIdSpec
        : Specification<Product>
    {
        public GetBaseProductByIdSpec(Guid id,
            bool asTracking = true,
            bool ignoreQueryFilter = true)
        {
            Query.Where(x => x.Id == id)
                .AsTracking(true)
                .IgnoreQueryFilters(ignoreQueryFilter);
        }
    }
}
