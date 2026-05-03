using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Common.Query;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products.AdminListProducts;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminListProducts
{
    public record AdminListProductsQuery
        : DateRangesSortedPagedIncludeDeletedQuery,
        IRequest<ErrorOr<PagedList<AdminListProductsResponse>>>
    {
        public string? SearchTerm { get; set; }
        public string? Status { get; set; } = null;
        public decimal? MinPrice { get; set; } = null;
        public decimal? MaxPrice { get; set; } = null;
    }
}
