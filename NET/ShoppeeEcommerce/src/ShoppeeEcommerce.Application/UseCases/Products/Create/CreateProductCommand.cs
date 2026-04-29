using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Products.Create;

namespace ShoppeeEcommerce.Application.UseCases.Products.Create
{
    public record CreateProductCommand(
        string Name,
        string Description,
        decimal Price,
        string SKU,
        List<Guid> CategoryIds,
        Guid UserId)
        : IRequest<ErrorOr<CreateProductResponse>>;
}
