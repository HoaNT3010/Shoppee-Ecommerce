using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.ListProducts
{
    public record ListProductsQuery(
        string? SearchTerm,
        decimal? MinPrice,
        decimal? MaxPrice,
        List<Guid>? CategoryIds,
        bool? IsFeatured,
        string? SortBy,
        bool? SortDesc,
        int? PageIndex,
        int? PageSize)
        : IRequest<ErrorOr<PagedList<ListProductResponse>>>;
}
