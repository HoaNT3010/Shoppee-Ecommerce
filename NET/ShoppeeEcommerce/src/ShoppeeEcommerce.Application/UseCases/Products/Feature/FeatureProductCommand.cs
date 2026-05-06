using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Products.Feature
{
    public record FeatureProductCommand(
        Guid Id)
        : IRequest<ErrorOr<Updated>>;
}
