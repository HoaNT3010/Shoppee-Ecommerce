using Ardalis.Specification;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;
using ShoppeeEcommerce.SharedViewModels.Models.Users;

namespace ShoppeeEcommerce.Application.UseCases.Categories.GetById
{
    internal class GetCategoryWithCreatorByIdSpec
        : Specification<Category, DetailedCategoryResponse>
    {
        public GetCategoryWithCreatorByIdSpec(
            Guid categoryId,
            bool asTracking = true,
            bool ignoreQueryFilter = true)
        {
            Query.Where(c => c.Id == categoryId)
                .Include(c => c.Creator)
                .AsTracking(asTracking)
                .IgnoreQueryFilters(ignoreQueryFilter)
                .Select(c => new DetailedCategoryResponse(
                    c.Id,
                    c.Name,
                    c.Description,
                    c.CreatedDate,
                    c.UpdatedDate,
                    c.IsDeleted,
                    c.DeletedDate,
                    c.Creator == null ? null : new BaseCreatorResponse(
                        c.Creator.Id,
                        c.Creator.UserName!,
                        c.Creator.Email!,
                        c.Creator.FirstName,
                        c.Creator.LastName)));
        }
    }
}
