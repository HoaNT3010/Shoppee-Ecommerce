using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products.AdminListProducts;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminListProducts
{
    internal class AdminListProductsQueryHandler(
        IRepository<Product, Guid> repo)
        : IRequestHandler<AdminListProductsQuery, ErrorOr<PagedList<AdminListProductsResponse>>>
    {
        public async Task<ErrorOr<PagedList<AdminListProductsResponse>>> Handle(
            AdminListProductsQuery request,
            CancellationToken cancellationToken)
        {
            var filterSpec = new AdminListProductsFilterSpec(request);
            var pagingSpec = new AdminListProductsPagingSpec(request);

            var totalItems = await repo.CountAsync(filterSpec, cancellationToken);
            var items = await repo.ListAsync(pagingSpec, cancellationToken);
            return PagedList.Create(items, totalItems, request.PageIndex!.Value, request.PageSize!.Value);
        }
    }
}
