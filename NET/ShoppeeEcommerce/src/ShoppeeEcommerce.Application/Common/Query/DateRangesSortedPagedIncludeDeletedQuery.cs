using MediatR;

namespace ShoppeeEcommerce.Application.Common.Query
{
    public record DateRangesSortedPagedIncludeDeletedQuery(
        DateTime? FromCreatedDate = null,
        DateTime? ToCreatedDate = null)
        : SortedPagedIncludeDeletedQuery, IRequest;
}
