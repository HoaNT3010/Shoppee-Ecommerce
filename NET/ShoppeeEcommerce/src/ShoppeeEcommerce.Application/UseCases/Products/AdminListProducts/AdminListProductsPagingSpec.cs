using ShoppeeEcommerce.Application.Common.Specifications;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminListProducts
{
    internal class AdminListProductsPagingSpec
        : AdminListProductsFilterSpec
    {
        public AdminListProductsPagingSpec(AdminListProductsQuery query) : base(query)
        {
            Query.ApplySorting(query.SortBy, query.SortDesc);
            Query.ApplyPaging(query.PageIndex ?? 1, query.PageSize ?? 10);
        }
    }
}
