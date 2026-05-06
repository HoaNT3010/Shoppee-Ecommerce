using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.ListProducts
{
    internal class ListProductsFilterSpec
        : Specification<Product, ListProductResponse>
    {
        public ListProductsFilterSpec(ListProductsQuery query)
        {
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                Query
                    .Search(x => x.Name, "%" + query.SearchTerm + "%")
                    .Search(x => x.Description, "%" + query.SearchTerm + "%")
                    .Search(x => x.SKU, "%" + query.SearchTerm + "%");
            }
            if (query.MinPrice.HasValue)
                Query.Where(x => x.Price >= query.MinPrice);
            if (query.MaxPrice.HasValue)
                Query.Where(x => x.Price <= query.MaxPrice);
            if (query.IsFeatured.HasValue)
                Query.Where(x => x.IsFeatured == query.IsFeatured.Value);

            if (query.CategoryIds?.Count > 0)
                Query.Where(x => x.Categories.Any(c => query.CategoryIds.Contains(c.Id)));

            Query.Where(x => x.Status != Domain.Enums.ProductStatus.Draft)
                .AsNoTracking()
                .AsSplitQuery()
                .Select(p => new ListProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    Price = p.Price,
                    IsFeatured = p.IsFeatured,
                    ImgUrl = p.ProductImages
                    .Where(i => i.IsMain)
                    .Select(i => i.Url)
                    .FirstOrDefault() ?? string.Empty
                }); ;
        }
    }
}
