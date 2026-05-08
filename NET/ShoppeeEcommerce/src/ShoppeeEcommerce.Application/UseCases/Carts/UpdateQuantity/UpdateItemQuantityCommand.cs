using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Carts.UpdateQuantity
{
    public record UpdateItemQuantityCommand(
        Guid ProductId,
        int Quantity)
        : IRequest<ErrorOr<Updated>>;
}
