namespace ShoppeeEcommerce.Application.Common.Query
{
    public record DateRangesSortedPagedIncludeDeletedQuery
        : SortedPagedIncludeDeletedQuery
    {
        public DateTime? FromCreatedDate { get; init; }
        public DateTime? ToCreatedDate { get; init; }
    }
}
