using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Carts.DeleteItem
{
    public record DeleteCartItemCommand(
        Guid ProductId)
        : IRequest<ErrorOr<Deleted>>;
}
