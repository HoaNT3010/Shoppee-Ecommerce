using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminListOrders;

namespace ShoppeeEcommerce.Application.UseCases.Orders.AdminListOrders
{
    public record AdminListOrdersQuery(
        string? Status,
        decimal? MinPrice,
        decimal? MaxPrice,
        DateTime? FromCreatedDate,
        DateTime? ToCreatedDate,
        string? SortBy,
        bool? SortDesc,
        int? PageIndex,
        int? PageSize)
        : IRequest<ErrorOr<PagedList<AdminListOrdersResponse>>>;
}
