using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminGetById
{
    internal class AdminProductDetailsSpec
        : Specification<Product>
    {
        public AdminProductDetailsSpec(Guid id)
        {
            Query.Where(x => x.Id == id)
                .Include(x => x.ProductImages)
                .Include(x => x.Categories)
                .Include(x => x.Creator)
                .AsNoTracking()
                .AsSplitQuery()
                .IgnoreQueryFilters();
        }
    }
}
