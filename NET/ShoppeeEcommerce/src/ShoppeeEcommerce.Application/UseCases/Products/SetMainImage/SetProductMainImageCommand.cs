using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Products.SetMainImage
{
    public record SetProductMainImageCommand(
        Guid ProductId,
        int ImageId)
        : IRequest<ErrorOr<Updated>>;
}
