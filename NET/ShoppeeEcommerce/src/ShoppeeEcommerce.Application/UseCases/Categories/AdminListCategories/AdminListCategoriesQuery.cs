using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Common.Query;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.AdminListCategories;
using ShoppeeEcommerce.SharedViewModels.Models.Common;

namespace ShoppeeEcommerce.Application.UseCases.Categories.AdminListCategories
{
    public record AdminListCategoriesQuery
        : DateRangesSortedPagedIncludeDeletedQuery,
        IRequest<ErrorOr<PagedList<AdminListCategoryResponse>>>
    {
        public string? SearchTerm { get; init; }
    }
}
