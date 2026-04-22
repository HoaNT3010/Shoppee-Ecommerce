using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;

namespace ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications
{
    internal class GetAllCategoriesSpec
        : Specification<Category, BaseCategoryResponse>
    {
        public GetAllCategoriesSpec(bool asTracking = false)
        {
            Query.AsTracking(asTracking)
                .Select(c => new BaseCategoryResponse(c.Id, c.Name, c.Description));
        }
    }
}
