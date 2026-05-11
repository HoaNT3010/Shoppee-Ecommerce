using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Orders.Cancel
{
    public record CancelOrderCommand(
        Guid OrderId,
        Guid UserId)
        : IRequest<ErrorOr<Updated>>;
}
