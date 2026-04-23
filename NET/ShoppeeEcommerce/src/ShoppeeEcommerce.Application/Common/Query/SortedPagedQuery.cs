using MediatR;

namespace ShoppeeEcommerce.Application.Common.Query
{
    public record SortedPagedQuery(
        string? SortBy = null,
        bool SortDesc = false)
        : PagedQuery, IRequest;
}
