using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminDetail;

namespace ShoppeeEcommerce.Application.UseCases.Orders.AdminViewDetail
{
    public record AdminViewOrderDetailQuery(
        Guid OrderId)
        : IRequest<ErrorOr<AdminViewOrderDetailResponse>>;
}
