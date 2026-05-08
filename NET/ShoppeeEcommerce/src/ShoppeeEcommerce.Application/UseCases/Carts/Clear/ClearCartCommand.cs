using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Carts.Clear
{
    public record ClearCartCommand()
        : IRequest<ErrorOr<Updated>>;
}
