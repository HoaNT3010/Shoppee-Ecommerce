using MediatR;

namespace ShoppeeEcommerce.Application.Common.Query
{
    public record SortedPagedIncludeDeletedQuery(
        bool IncludeDeleted = false)
        : SortedPagedQuery, IRequest;
}
