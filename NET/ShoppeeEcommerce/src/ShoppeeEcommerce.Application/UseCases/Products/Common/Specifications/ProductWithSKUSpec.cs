using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using System.Xml.Linq;

namespace ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications
{
    internal class ProductWithSKUSpec
        : Specification<Product>
    {
        public ProductWithSKUSpec(
            string sku,
            bool asTracking = false,
            bool ignoreQueryFilter = true)
        {
            Query.Where(p => p.SKU.ToLower() == sku.ToLower())
                .AsTracking(asTracking)
                .IgnoreQueryFilters(ignoreQueryFilter);
        }
    }
}
