using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.GetNewest
{
    internal class GetNewestProductsQueryHandler(
        IRepository<Product, Guid> repo)
        : IRequestHandler<GetNewestProductsQuery, ErrorOr<List<BaseProductSummaryResponse>>>
    {
        public async Task<ErrorOr<List<BaseProductSummaryResponse>>> Handle(
            GetNewestProductsQuery request,
            CancellationToken cancellationToken)
        {
            return await repo.ListAsync(
                new GetNewestProductsSpec(request.Count),
                cancellationToken);
        }
    }
}
