using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.GetFeatured
{
    internal class GetFeaturedProductsSpec
        : Specification<Product, BaseProductSummaryResponse>
    {
        public GetFeaturedProductsSpec(int count)
        {
            Query.Where(p => p.Status != Domain.Enums.ProductStatus.Draft && p.IsFeatured)
                // Default sort from latest -> oldest
                .OrderByDescending(p => p.CreatedDate)
                .Take(count)
                .AsNoTracking()
                .AsSplitQuery()
                .Select(p => new BaseProductSummaryResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    Price = p.Price,
                    ImgUrl = p.ProductImages
                        .Where(i => i.IsMain)
                        .Select(i => i.Url)
                        .FirstOrDefault() ?? string.Empty
                });
        }
    }
}
