using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;

namespace ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications
{
    internal class PublicProductByIdSpec
        : Specification<Product>
    {
        public PublicProductByIdSpec(Guid id, bool includeImages = true, bool includeCategories = true)
        {
            if (includeImages) Query.Include(x => x.ProductImages);
            if (includeCategories) Query.Include(x => x.Categories).AsSplitQuery();
            Query.Where(x => x.Id == id && x.Status != ProductStatus.Draft)
                .AsNoTracking();
        }
    }
}
