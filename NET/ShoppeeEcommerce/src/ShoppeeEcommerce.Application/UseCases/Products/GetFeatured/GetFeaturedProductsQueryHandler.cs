using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.GetFeatured
{
    internal class GetFeaturedProductsQueryHandler(
        IRepository<Product, Guid> repo)
        : IRequestHandler<GetFeaturedProductsQuery, ErrorOr<List<BaseProductSummaryResponse>>>
    {
        public async Task<ErrorOr<List<BaseProductSummaryResponse>>> Handle(GetFeaturedProductsQuery request, CancellationToken cancellationToken)
        {
            return await repo.ListAsync(
                new GetFeaturedProductsSpec(request.Count),
                cancellationToken);
        }
    }
}
