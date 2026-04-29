using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications
{
    internal class ProductWithNameSpec
        : Specification<Product>
    {
        public ProductWithNameSpec(
            string name,
            bool asTracking = false,
            bool ignoreQueryFilter = true)
        {
            Query.Where(p => p.Name.ToLower() == name.ToLower())
                .AsTracking(asTracking)
                .IgnoreQueryFilters(ignoreQueryFilter);
        }
    }
}
