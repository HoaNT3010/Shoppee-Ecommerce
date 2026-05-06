using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.ListProducts
{
    internal class ListProductsQueryHandler(
        IRepository<Product, Guid> repo)
        : IRequestHandler<ListProductsQuery, ErrorOr<PagedList<ListProductResponse>>>
    {
        public async Task<ErrorOr<PagedList<ListProductResponse>>> Handle(
            ListProductsQuery request,
            CancellationToken cancellationToken)
        {
            var filterSpec = new ListProductsFilterSpec(request);
            var pagingSpec = new ListProductsPagingSpec(request);

            var totalItems = await repo.CountAsync(filterSpec, cancellationToken);
            var items = await repo.ListAsync(pagingSpec, cancellationToken);
            return PagedList.Create(items, totalItems, request.PageIndex!.Value, request.PageSize!.Value);
        }
    }
}
