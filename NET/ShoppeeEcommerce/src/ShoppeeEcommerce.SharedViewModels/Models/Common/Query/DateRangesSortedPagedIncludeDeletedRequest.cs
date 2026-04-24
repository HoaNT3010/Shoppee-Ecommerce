namespace ShoppeeEcommerce.SharedViewModels.Models.Common.Query
{
    public record DateRangesSortedPagedIncludeDeletedRequest
        : SortedPagedIncludeDeletedRequest
    {
        public DateTime? FromCreatedDate { get; init; }
        public DateTime? ToCreatedDate { get; init; }
    }
}
