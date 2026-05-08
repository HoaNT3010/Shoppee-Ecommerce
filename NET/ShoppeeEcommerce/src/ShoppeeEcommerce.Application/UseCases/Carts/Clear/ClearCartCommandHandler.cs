using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.Services;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Carts.Clear
{
    internal class ClearCartCommandHandler(
        ICartService cartService)
        : IRequestHandler<ClearCartCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await cartService.ClearAsync(cancellationToken);
                return Result.Updated;
            }
            catch
            {
                return Errors.CartErrors.ClearItemsFailed();
            }
        }
    }
}
