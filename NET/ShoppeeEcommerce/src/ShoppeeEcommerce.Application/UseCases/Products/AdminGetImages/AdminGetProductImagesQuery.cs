using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminGetImages
{
    public record AdminGetProductImagesQuery(
        Guid Id)
        : IRequest<ErrorOr<List<BaseProductImageResponse>>>;
}
