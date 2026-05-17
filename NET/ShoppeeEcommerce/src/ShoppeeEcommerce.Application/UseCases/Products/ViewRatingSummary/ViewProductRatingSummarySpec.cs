using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;

namespace ShoppeeEcommerce.Application.UseCases.Products.ViewRatingSummary
{
    internal class ViewProductRatingSummarySpec
        : Specification<Product>
    {
        public ViewProductRatingSummarySpec(Guid productId, bool isAdmin = false)
        {
            Query.Where(p => p.Id == productId)
                .Include(p => p.RatingSummary)
                .AsNoTracking();
            if (isAdmin)
            {
                Query.IgnoreQueryFilters();
            }
            else
            {
                Query.Where(p => p.Status == ProductStatus.Published);
            }
        }
    }
}
