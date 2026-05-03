using Ardalis.Specification;
using ShoppeeEcommerce.Application.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.SharedViewModels.Models.Products.AdminListProducts;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminListProducts
{
    internal class AdminListProductsFilterSpec
        : Specification<Product, AdminListProductsResponse>
    {
        public AdminListProductsFilterSpec(AdminListProductsQuery query)
        {
            Query.Include(x => x.ProductImages.Where(i => i.IsMain));
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                Query
                    .Search(x => x.Name, "%" + query.SearchTerm + "%")
                    .Search(x => x.Description, "%" + query.SearchTerm + "%")
                    .Search(x => x.SKU, "%" + query.SearchTerm + "%");
            }
            // Status
            if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse(query.Status, out ProductStatus status))
            {
                Query.Where(x => x.Status == status);
            }
            // Price Range
            if (query.MinPrice.HasValue)
                Query.Where(x =>
                    x.Price >= query.MinPrice);
            if (query.MaxPrice.HasValue)
                Query.Where(x =>
                    x.Price <= query.MaxPrice);
            Query.ApplyCommonFilters(query);
            Query.Select(x => new AdminListProductsResponse(
                x.Id,
                x.Name,
                x.Description,
                x.Price,
                x.SKU,
                x.CreatedDate,
                x.IsDeleted,
                x.Status.ToString(),
                x.ProductImages
                .Where(i => i.IsMain)
                .Select(i => i.Url)
                .FirstOrDefault()));
        }
    }
}
