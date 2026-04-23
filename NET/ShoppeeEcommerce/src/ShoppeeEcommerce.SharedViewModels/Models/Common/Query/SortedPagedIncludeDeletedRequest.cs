namespace ShoppeeEcommerce.SharedViewModels.Models.Common.Query
{
    public record SortedPagedIncludeDeletedRequest(
        bool IncludeDeleted = false)
        : SortedPagedRequest;
}
