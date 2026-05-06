using ShoppeeEcommerce.Application.Common.Specifications;

namespace ShoppeeEcommerce.Application.UseCases.Products.ListProducts
{
    internal class ListProductsPagingSpec
        : ListProductsFilterSpec
    {
        public ListProductsPagingSpec(ListProductsQuery query) : base(query)
        {
            Query.ApplySorting(query.SortBy, query.SortDesc);
            Query.ApplyPaging(query.PageIndex ?? 1, query.PageSize ?? 10);
        }
    }
}
