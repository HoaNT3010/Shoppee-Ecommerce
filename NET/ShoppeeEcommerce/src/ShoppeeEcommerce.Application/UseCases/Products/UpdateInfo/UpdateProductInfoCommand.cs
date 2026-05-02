using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Products.UpdateInfo
{
    public record UpdateProductInfoCommand(
        Guid Id,
        string? Name,
        string? Description,
        decimal? Price,
        string? SKU)
        : IRequest<ErrorOr<Updated>>;
}
