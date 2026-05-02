using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Storage;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Products;
using ShoppeeEcommerce.SharedViewModels.Models.Products.UploadImages;

namespace ShoppeeEcommerce.Application.UseCases.Products.UploadImages
{
    internal class UploadProductImagesCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        IFileService fs,
        ILogger<UploadProductImagesCommandHandler> logger)
        : IRequestHandler<UploadProductImagesCommand, ErrorOr<UploadProductImagesResponse>>
    {
        public async Task<ErrorOr<UploadProductImagesResponse>> Handle(
            UploadProductImagesCommand request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new ProductByIdSpec(
                    request.ProductId,
                    includeImages: true,
                    asTracking: true,
                    ignoreQueryFilter: true,
                    onlyActiveProduct: false), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.ProductId.ToString());
            // Validate product images count (current + with new images)
            int existingCount = product.ProductImages.Count;
            if (existingCount >= 10 || existingCount + request.Images.Count > 10)
                return Errors.ProductErrors.ExceedMaximumImages(10);

            var streams = request.Images.Select(f => f.OpenReadStream()).ToList();
            try
            {
                var fileStreams = request.Images.Select((f, i) =>
                (
                    Stream: streams[i],
                     f.FileName
                ));
                var uploadResults = await fs.UploadManyAsync(
                    fileStreams,
                    folder: $"product/{request.ProductId.ToString()}",
                    cancellationToken: cancellationToken);

                var images = uploadResults.Select((result, index) =>
                new ProductImage()
                {
                    ProductId = request.ProductId,
                    Url = result.Url,
                    PublicId = result.PublicId,
                    DisplayOrder = existingCount + index + 1,
                    IsMain = false,
                    AltText = $"Image number {existingCount + index + 1} of product '{product.Name}'.",
                }).ToList();
                product.ProductImages.AddRange(images);
                await uow.SaveChangesAsync(cancellationToken);
                return new UploadProductImagesResponse(images
                    .Select(
                        i => new CreateProductImageResponse(i.Id, i.Url, i.DisplayOrder, i.IsMain))
                    .ToList());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred when trying to upload images for product '{ProductId}': {ExceptionMsg}", request.ProductId, ex.Message);
                return Errors.ProductErrors.UpdateImagesFailed(request.ProductId.ToString());
            }
            finally
            {
                foreach (var stream in streams)
                    await stream.DisposeAsync();
            }
        }
    }
}
