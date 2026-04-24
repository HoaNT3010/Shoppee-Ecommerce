namespace ShoppeeEcommerce.Application.Common.Query
{
    public record SortedPagedQuery
        : PagedQuery
    {
        public string? SortBy { get; init; }
        public bool? SortDesc { get; init; } = false;
    }
}
