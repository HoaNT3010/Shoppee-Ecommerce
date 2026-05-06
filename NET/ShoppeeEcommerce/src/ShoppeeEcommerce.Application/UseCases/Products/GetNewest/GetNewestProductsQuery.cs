using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.GetNewest
{
    public record GetNewestProductsQuery(
        int Count = 8)
        : IRequest<ErrorOr<List<BaseProductSummaryResponse>>>;
}
