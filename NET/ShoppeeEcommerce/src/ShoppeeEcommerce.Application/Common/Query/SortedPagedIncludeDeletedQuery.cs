namespace ShoppeeEcommerce.Application.Common.Query
{
    public record SortedPagedIncludeDeletedQuery
        : SortedPagedQuery
    {
        public bool? IncludeDeleted { get; init; } = false;
    }
}
