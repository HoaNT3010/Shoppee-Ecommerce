namespace ShoppeeEcommerce.SharedViewModels.Models.Common.Query
{
    public record SortedPagedIncludeDeletedRequest
        : SortedPagedRequest
    {
        public bool? IncludeDeleted { get; init; } = false;
    }
}
