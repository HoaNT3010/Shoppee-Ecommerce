using ShoppeeEcommerce.Application.Common.Specifications;

namespace ShoppeeEcommerce.Application.UseCases.Categories.AdminListCategories
{
    internal class AdminListCategoriesPagingSpecification
        : AdminListCategoriesFilterSpecification
    {
        public AdminListCategoriesPagingSpecification(
            AdminListCategoriesQuery query)
            : base(query)
        {
            Query.ApplySorting(query.SortBy, query.SortDesc);
            Query.ApplyPaging(query.PageIndex ?? 1, query.PageSize ?? 10);
        }
    }
}
