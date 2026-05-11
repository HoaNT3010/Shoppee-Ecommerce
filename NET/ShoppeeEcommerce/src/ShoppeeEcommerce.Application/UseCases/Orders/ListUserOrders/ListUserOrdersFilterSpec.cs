using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Orders.ListUserOrders
{
    internal class ListUserOrdersFilterSpec
        : Specification<Order, UserOrderSummary>
    {
        public ListUserOrdersFilterSpec(ListUserOrdersQuery request)
        {
            Query.Where(o => o.UserId == request.UserId);
            if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse(request.Status, out OrderStatus status))
                Query.Where(o => o.Status == status);
            Query
                .AsNoTracking()
                .AsSplitQuery()
                .Select(o => new UserOrderSummary
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    Status = o.Status.ToString(),
                    TotalPrice = o.TotalPrice,
                    CreatedDate = o.CreatedDate,
                    UpdatedDate = o.UpdatedDate,
                    LineItemsCount = o.Items.Count
                });
        }
    }
}
