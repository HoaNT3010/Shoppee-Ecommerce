using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using ShoppeeEcommerce.SharedViewModels.Models.Products.UploadImages;

namespace ShoppeeEcommerce.Application.UseCases.Products.UploadImages
{
    public record UploadProductImagesCommand(
        IFormFileCollection Images,
        Guid ProductId)
        : IRequest<ErrorOr<UploadProductImagesResponse>>;
}
