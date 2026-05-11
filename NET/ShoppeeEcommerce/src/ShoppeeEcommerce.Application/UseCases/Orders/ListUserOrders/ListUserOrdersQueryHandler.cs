using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;

namespace ShoppeeEcommerce.Application.UseCases.Orders.ListUserOrders
{
    internal class ListUserOrdersQueryHandler(
        IRepository<Order, Guid> repo)
        : IRequestHandler<ListUserOrdersQuery, ErrorOr<PagedList<UserOrderSummary>>>
    {
        public async Task<ErrorOr<PagedList<UserOrderSummary>>> Handle(ListUserOrdersQuery request, CancellationToken cancellationToken)
        {
            var filterSpec = new ListUserOrdersFilterSpec(request);
            var pagingSpec = new ListUserOrdersPagingSpec(request);

            var totalItems = await repo.CountAsync(filterSpec, cancellationToken);
            var items = await repo.ListAsync(pagingSpec, cancellationToken);
            return PagedList.Create(items, totalItems, request.PageIndex!.Value, request.PageSize!.Value);
        }
    }
}
