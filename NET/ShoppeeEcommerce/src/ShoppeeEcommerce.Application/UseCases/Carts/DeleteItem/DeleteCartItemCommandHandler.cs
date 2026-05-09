using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Carts.DeleteItem
{
    internal class DeleteCartItemCommandHandler(
        ICartService cartService)
        : IRequestHandler<DeleteCartItemCommand, ErrorOr<Deleted>>
    {
        public async Task<ErrorOr<Deleted>> Handle(DeleteCartItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await cartService.RemoveItemAsync(request.ProductId, cancellationToken);
                return Result.Deleted;
            }
            catch
            {
                return Errors.CartErrors.RemoveItemFailed();
            }
        }
    }
}
