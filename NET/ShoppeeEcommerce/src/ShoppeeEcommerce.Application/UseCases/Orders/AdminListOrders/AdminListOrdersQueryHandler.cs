using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminListOrders;

namespace ShoppeeEcommerce.Application.UseCases.Orders.AdminListOrders
{
    internal class AdminListOrdersQueryHandler(
        IRepository<Order, Guid> repo)
        : IRequestHandler<AdminListOrdersQuery, ErrorOr<PagedList<AdminListOrdersResponse>>>
    {
        public async Task<ErrorOr<PagedList<AdminListOrdersResponse>>> Handle(AdminListOrdersQuery request, CancellationToken cancellationToken)
        {
            var filterSpec = new AdminListOrdersFilterSpec(request);
            var pagingSpec = new AdminListOrdersPagingSpec(request);

            var totalItems = await repo.CountAsync(filterSpec, cancellationToken);
            var items = await repo.ListAsync(pagingSpec, cancellationToken);
            return PagedList.Create(items, totalItems, request.PageIndex!.Value, request.PageSize!.Value);
        }
    }
}
