using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Orders.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Enums;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Orders.Cancel
{
    internal class CancelOrderCommandHandler(
        IRepository<Order, Guid> repo,
        IUnitOfWork uow,
        ILogger<CancelOrderCommandHandler> logger)
        : IRequestHandler<CancelOrderCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await repo.FirstOrDefaultAsync(new OrderByIdSpec(request.OrderId), cancellationToken);
            if (order == null) return Errors.OrderErrors.NotFoundWithId(request.OrderId.ToString());
            if (order.UserId != request.UserId) return Errors.OrderErrors.UserNotMatchOwner();

            // Idempotency: If order is cancelled, return successful
            if (order.Status == OrderStatus.Cancelled) return Result.Updated;

            // Only PENDING order can be cancelled
            if (order.Status != OrderStatus.Pending) return Errors.OrderErrors.CancelInvalidStatus();
            try
            {
                order.CancelOrder();
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Updated;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred when trying to cancel order with ID '{OrderId}': {ExMsg}", request.OrderId, ex.Message);
                return Errors.OrderErrors.CancelOrderFailed();
            }
        }
    }
}
