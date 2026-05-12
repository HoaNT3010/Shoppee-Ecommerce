using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Payments.Common.Specification
{
    public class PaymentByIdSpec
        : Specification<Payment>
    {
        public PaymentByIdSpec(Guid paymentId, bool includeOrder = true, bool asTracking = true)
        {
            Query.Where(p => p.Id == paymentId)
                .AsTracking(asTracking);
            if (includeOrder)
                Query.Include(p => p.Order);
        }
    }
}
