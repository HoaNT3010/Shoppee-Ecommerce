using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Products.PublishProduct
{
    public record PublishProductCommand(
        Guid Id)
        : IRequest<ErrorOr<Updated>>;
}
