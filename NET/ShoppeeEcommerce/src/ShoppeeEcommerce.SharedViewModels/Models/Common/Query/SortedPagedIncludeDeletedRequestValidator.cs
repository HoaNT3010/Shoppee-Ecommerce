namespace ShoppeeEcommerce.SharedViewModels.Models.Common.Query
{
    // Bad name
    public class SortedPagedIncludeDeletedRequestValidator<T>
        : SortedPagedRequestValidator<T>
        where T : SortedPagedIncludeDeletedRequest
    {
        public SortedPagedIncludeDeletedRequestValidator(IReadOnlyCollection<string> allowedSortFields)
            : base(allowedSortFields)
        {
        }
    }
}
