using ShoppeeEcommerce.Application.Common.Specifications;

namespace ShoppeeEcommerce.Application.UseCases.Orders.AdminListOrders
{
    internal class AdminListOrdersPagingSpec
        : AdminListOrdersFilterSpec
    {
        public AdminListOrdersPagingSpec(AdminListOrdersQuery request) : base(request)
        {
            Query.ApplySorting(request.SortBy, request.SortDesc);
            Query.ApplyPaging(request.PageIndex ?? 1, request.PageSize ?? 10);
        }
    }
}
