using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Carts.Common.Specifications
{
    internal class CartWithItemsSpec
        : Specification<Cart>
    {
        public CartWithItemsSpec(Guid? userId = null, string? sessionId = null, bool asTracking = true)
        {
            if (userId != null)
                Query.Where(c => c.UserId == userId);
            if (sessionId != null)
                Query.Where(c => c.SessionId == sessionId);
            Query.Include(c => c.CartItems)
                .AsTracking(asTracking);
        }
    }
}
