using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminDetail;

namespace ShoppeeEcommerce.Application.UseCases.Orders.AdminViewDetail
{
    internal class AdminViewOrderDetailQueryHandler(
        IRepository<Order, Guid> repo)
        : IRequestHandler<AdminViewOrderDetailQuery, ErrorOr<AdminViewOrderDetailResponse>>
    {
        public async Task<ErrorOr<AdminViewOrderDetailResponse>> Handle(AdminViewOrderDetailQuery request, CancellationToken cancellationToken)
        {
            var order = await repo.FirstOrDefaultAsync(new AdminViewOrderDetailSpec(request.OrderId), cancellationToken);
            if (order is null) return Errors.OrderErrors.NotFoundWithId(request.OrderId.ToString());
            return order;
        }
    }
}
