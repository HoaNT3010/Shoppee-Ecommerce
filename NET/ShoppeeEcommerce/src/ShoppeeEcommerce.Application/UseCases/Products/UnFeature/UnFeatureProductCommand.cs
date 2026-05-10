using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Products.UnFeature
{
    public record UnFeatureProductCommand(
        Guid Id)
        : IRequest<ErrorOr<Updated>>;
}
