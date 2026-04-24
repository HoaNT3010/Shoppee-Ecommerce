namespace ShoppeeEcommerce.SharedViewModels.Models.Common.Query
{
    public record SortedPagedRequest
        : PagedRequest
    {
        public string? SortBy { get; init; }
        public bool? SortDesc { get; init; } = false;
    }
}
