using ShoppeeEcommerce.SharedViewModels.Models.Common.Query;

namespace ShoppeeEcommerce.WebAPI.Common.Validators.Query
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
