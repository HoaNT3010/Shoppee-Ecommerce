using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Products.UpdateCategories
{
    public record UpdateProductCategoriesCommand(
        Guid Id,
        List<Guid> CategoryIds)
        : IRequest<ErrorOr<Updated>>;
}
