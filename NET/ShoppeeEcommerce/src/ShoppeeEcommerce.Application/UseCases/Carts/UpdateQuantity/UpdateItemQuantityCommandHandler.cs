using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Carts.UpdateQuantity
{
    internal class UpdateItemQuantityCommandHandler(
        ICartService cartService)
        : IRequestHandler<UpdateItemQuantityCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await cartService.UpdateQuantityAsync(request.ProductId, request.Quantity, cancellationToken);
                return Result.Updated;
            }
            catch
            {
                return Errors.CartErrors.UpdateItemQuantityFailed();
            }
        }
    }
}
