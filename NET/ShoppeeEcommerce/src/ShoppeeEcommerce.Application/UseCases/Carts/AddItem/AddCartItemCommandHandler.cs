using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Carts.AddItem
{
    internal class AddCartItemCommandHandler(
        ICartService cartService)
        : IRequestHandler<AddCartItemCommand, ErrorOr<Created>>
    {
        public async Task<ErrorOr<Created>> Handle(
            AddCartItemCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await cartService.AddItemAsync(request.ProductId, request.Quantity, cancellationToken);
                return Result.Created;
            }
            catch
            {
                return Errors.CartErrors.AddItemFailed();
            }
        }
    }
}
