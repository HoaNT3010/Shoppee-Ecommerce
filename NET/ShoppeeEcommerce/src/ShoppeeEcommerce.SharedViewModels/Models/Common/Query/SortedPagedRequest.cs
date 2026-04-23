namespace ShoppeeEcommerce.SharedViewModels.Models.Common.Query
{
    public record SortedPagedRequest(
        string? SortBy = null,
        bool SortDesc = false)
        : PagedRequest;
}
