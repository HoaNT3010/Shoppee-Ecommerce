using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;

namespace ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications
{
    internal class CategoriesByIdsSpec
        : Specification<Category>
    {
        public CategoriesByIdsSpec(
            IEnumerable<Guid> Ids,
            bool asTracking = false)
        {
            Query.Where(c => Ids.Contains(c.Id) && !c.IsDeleted)
                .AsTracking(asTracking);
        }
    }
}
