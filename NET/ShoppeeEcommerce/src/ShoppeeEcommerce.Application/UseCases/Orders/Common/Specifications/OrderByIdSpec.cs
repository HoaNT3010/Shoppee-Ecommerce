using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Orders.Common.Specifications
{
    internal class OrderByIdSpec
        : Specification<Order>
    {
        public OrderByIdSpec(Guid orderId, bool includeItems = true, bool asTracking = true)
        {
            Query.Where(o => o.Id == orderId)
                .AsTracking(asTracking);
            if (includeItems)
                Query.Include(o => o.Items);
        }
    }
}
