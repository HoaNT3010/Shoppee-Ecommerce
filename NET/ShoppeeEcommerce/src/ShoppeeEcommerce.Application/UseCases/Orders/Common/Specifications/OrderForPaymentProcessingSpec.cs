using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Orders.Common.Specifications
{
    public class OrderForPaymentProcessingSpec
        : Specification<Order>
    {
        public OrderForPaymentProcessingSpec(Guid orderId, Guid userId,
            bool includeItems = true,
            bool asTracking = true)
        {
            Query.Where(o => o.Id == orderId && o.UserId == userId)
                .Include(o => o.Payment)
                .AsTracking(asTracking)
                .AsSplitQuery();
            if (includeItems) Query.Include(o => o.Items);
        }
    }
}
