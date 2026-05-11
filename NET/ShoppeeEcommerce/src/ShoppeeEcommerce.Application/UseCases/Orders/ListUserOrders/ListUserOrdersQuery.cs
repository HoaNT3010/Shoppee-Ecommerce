using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;

namespace ShoppeeEcommerce.Application.UseCases.Orders.ListUserOrders
{
    public record ListUserOrdersQuery(
        Guid UserId,
        string? Status,
        string? SortBy,
        bool? SortDesc,
        int? PageIndex,
        int? PageSize)
        : IRequest<ErrorOr<PagedList<UserOrderSummary>>>;
}
