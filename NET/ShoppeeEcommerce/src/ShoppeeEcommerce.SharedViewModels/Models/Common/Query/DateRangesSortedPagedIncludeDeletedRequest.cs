namespace ShoppeeEcommerce.SharedViewModels.Models.Common.Query
{
    public record DateRangesSortedPagedIncludeDeletedRequest(
        DateTime? FromCreatedDate = null,
        DateTime? ToCreatedDate = null)
        : SortedPagedIncludeDeletedRequest;
}
