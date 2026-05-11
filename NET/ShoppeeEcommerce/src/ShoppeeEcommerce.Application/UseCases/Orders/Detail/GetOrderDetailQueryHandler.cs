using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.Detail;

namespace ShoppeeEcommerce.Application.UseCases.Orders.Detail
{
    internal class GetOrderDetailQueryHandler(
        IRepository<Order, Guid> repo)
        : IRequestHandler<GetOrderDetailQuery, ErrorOr<OrderDetailResponse>>
    {
        public async Task<ErrorOr<OrderDetailResponse>> Handle(GetOrderDetailQuery request, CancellationToken cancellationToken)
        {
            var order = await repo.FirstOrDefaultAsync(new GetOrderDetailSpec(request.OrderId), cancellationToken);
            if (order == null) return Errors.OrderErrors.NotFoundWithId(request.OrderId.ToString());
            return order;
        }
    }
}
