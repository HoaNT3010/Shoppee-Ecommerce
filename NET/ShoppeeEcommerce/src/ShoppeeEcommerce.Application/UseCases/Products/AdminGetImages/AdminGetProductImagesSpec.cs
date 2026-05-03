using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminGetImages
{
    internal class AdminGetProductImagesSpec
        : Specification<Product>
    {
        public AdminGetProductImagesSpec(Guid id)
        {
            Query.Where(x => x.Id == id)
                .Include(x => x.ProductImages)
                .AsNoTracking()
                .IgnoreQueryFilters();
        }
    }
}
