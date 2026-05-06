using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.GetNewest
{
    internal class GetNewestProductsSpec
        : Specification<Product, BaseProductSummaryResponse>
    {
        public GetNewestProductsSpec(int count)
        {
            Query.Where(p => p.Status != Domain.Enums.ProductStatus.Draft)
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
