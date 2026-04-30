using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;

namespace ShoppeeEcommerce.Application.UseCases.Products.GetById
{
    internal class ProductDetailsSpec
        : Specification<Product>
    {
        public ProductDetailsSpec(Guid id)
        {
            Query.Where(x => x.Id == id && x.Status == ProductStatus.Published)
                .Include(x => x.ProductImages)
                .Include(x => x.Categories)
                .AsNoTracking()
                .AsSplitQuery();
        }
    }
}
