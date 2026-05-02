using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ReorderImages;

namespace ShoppeeEcommerce.Application.UseCases.Products.ReorderImages
{
    public record ReorderProductImagesCommand(
        Guid Id,
        List<ProductImageItem> Orders)
        : IRequest<ErrorOr<Updated>>;
}
