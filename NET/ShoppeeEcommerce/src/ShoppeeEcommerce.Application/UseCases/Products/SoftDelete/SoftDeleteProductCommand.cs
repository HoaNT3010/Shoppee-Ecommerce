using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Products.SoftDelete
{
    public record SoftDeleteProductCommand(
        Guid Id)
        : IRequest<ErrorOr<Deleted>>;
}
