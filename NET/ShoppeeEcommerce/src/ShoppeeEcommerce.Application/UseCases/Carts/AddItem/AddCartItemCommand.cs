using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Carts.AddItem
{
    public record AddCartItemCommand(
        Guid ProductId,
        int Quantity = 1)
        : IRequest<ErrorOr<Created>>;
}
