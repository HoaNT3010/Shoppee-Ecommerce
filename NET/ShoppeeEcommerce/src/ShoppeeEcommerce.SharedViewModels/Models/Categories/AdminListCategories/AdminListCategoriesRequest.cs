using ShoppeeEcommerce.SharedViewModels.Models.Common.Query;

namespace ShoppeeEcommerce.SharedViewModels.Models.Categories.AdminListCategories
{
    public record AdminListCategoriesRequest
        : DateRangesSortedPagedIncludeDeletedRequest
    {
        public string? SearchTerm { get; set; }
    }
}
