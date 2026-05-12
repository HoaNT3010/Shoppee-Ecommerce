using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Payments.Common.Specification
{
    public class PaymentByIntentIdSpec
        : Specification<Payment>
    {
        public PaymentByIntentIdSpec(string intentId, bool includeOrder = true, bool asTracking = true)
        {
            Query.Where(p => p.StripePaymentIntentId == intentId)
                .AsTracking(asTracking);
            if (includeOrder)
                Query.Include(p => p.Order);
        }
    }
}
