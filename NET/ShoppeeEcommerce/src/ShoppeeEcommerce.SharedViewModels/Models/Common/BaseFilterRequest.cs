namespace ShoppeeEcommerce.SharedViewModels.Models.Common
{
    public record BaseFilterRequest
    {
        // Paging
        public int PageIndex { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        // Searching
        public string? SearchTerm { get; init; }
        // Sorting
        public string? SortBy { get; init; }
        public bool SortDesc { get; init; }
        // Soft delete
        public bool IncludeDeleted { get; init; }
        // Date range
        public DateTime? FromCreatedDate { get; init; }
        public DateTime? ToCreatedDate { get; init; }
    }
}
