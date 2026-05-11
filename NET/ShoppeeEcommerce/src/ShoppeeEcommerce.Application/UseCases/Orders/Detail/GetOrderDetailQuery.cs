using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.Detail;

namespace ShoppeeEcommerce.Application.UseCases.Orders.Detail
{
    public record GetOrderDetailQuery(
        Guid OrderId)
        : IRequest<ErrorOr<OrderDetailResponse>>;
}
