using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.GetFeatured
{
    public record GetFeaturedProductsQuery(
        int Count = 8)
        : IRequest<ErrorOr<List<BaseProductSummaryResponse>>>;
}
