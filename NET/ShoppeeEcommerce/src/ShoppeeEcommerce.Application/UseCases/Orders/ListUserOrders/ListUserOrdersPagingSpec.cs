using ShoppeeEcommerce.Application.Common.Specifications;

namespace ShoppeeEcommerce.Application.UseCases.Orders.ListUserOrders
{
    internal class ListUserOrdersPagingSpec
        : ListUserOrdersFilterSpec
    {
        public ListUserOrdersPagingSpec(ListUserOrdersQuery request) : base(request)
        {
            Query.ApplySorting(request.SortBy, request.SortDesc);
            Query.ApplyPaging(request.PageIndex ?? 1, request.PageSize ?? 10);
        }
    }
}
