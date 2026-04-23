using MediatR;

namespace ShoppeeEcommerce.Application.Common.Query
{
    public record PagedQuery(
        int PageIndex = 1,
        int PageSize = 10)
        : IRequest;
}
