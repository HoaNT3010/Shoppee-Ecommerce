using Ardalis.Specification;
using ShoppeeEcommerce.Application.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.AdminListCategories;
using System.Xml.Linq;

namespace ShoppeeEcommerce.Application.UseCases.Categories.AdminListCategories
{
    internal class AdminListCategoriesFilterSpecification
        : Specification<Category, AdminListCategoryResponse>
    {
        public AdminListCategoriesFilterSpecification(AdminListCategoriesQuery query)
        {
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                Query
                    .Search(x => x.Name, "%" + query.SearchTerm + "%")
                    .Search(x => x.Description, "%" + query.SearchTerm + "%");
            }
            Query.ApplyCommonFilters(query);
            Query.Select(x => new AdminListCategoryResponse(
                x.Id,
                x.Name,
                x.Description,
                x.CreatedDate,
                x.IsDeleted));
        }
    }
}
