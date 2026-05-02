using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Products.HardDelete
{
    public record HardDeleteProductCommand(
        Guid Id)
        : IRequest<ErrorOr<Deleted>>;
}
