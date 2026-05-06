using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.UnFeature
{
    public record UnFeatureProductCommand(
        Guid Id)
        : IRequest<ErrorOr<Updated>>;
}
