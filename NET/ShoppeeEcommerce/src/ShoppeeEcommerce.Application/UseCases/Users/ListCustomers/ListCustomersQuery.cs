using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Common.Query;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Users.ListCustomers;

namespace ShoppeeEcommerce.Application.UseCases.Users.ListCustomers
{
    public record ListCustomersQuery
        : PagedQuery,
        IRequest<ErrorOr<PagedList<ListCustomersResponse>>>
    {
        public string? SearchTerm { get; set; }
    }
}
