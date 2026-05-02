using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.ReorderImages
{
    internal class ReorderProductImagesCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        ILogger<ReorderProductImagesCommandHandler> logger)
        : IRequestHandler<ReorderProductImagesCommand, ErrorOr<Updated>>
    {
        const int Temp_Order_Offset = 10_000;

        public async Task<ErrorOr<Updated>> Handle(
            ReorderProductImagesCommand request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new ProductByIdSpec(
                    request.Id,
                    includeImages: true,
                    asTracking: true,
                    ignoreQueryFilter: true,
                    includeCategories: false,
                    includeCreator: false,
                    onlyActiveProduct: false),
                cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());

            if (product.ProductImages.Count == 0) return Errors.ProductErrors.HasNoImages;

            var requestIds = request.Orders.Select(x => x.ImageId).ToHashSet();
            var imageIds = product.ProductImages.Select(x => x.Id).ToHashSet();
            // Request must send all product images.
            var missingIds = imageIds.Except(requestIds).ToArray();
            if (missingIds.Length > 0) return Errors.ProductErrors.ImagesMissingFromReorder(missingIds);
            // Request must not contain images that not belong to product
            var invalidIds = requestIds.Except(imageIds).ToArray();
            if (invalidIds.Length > 0) return Errors.ProductErrors.ImagesNotFound(invalidIds);

            var orderLookup = request.Orders.ToDictionary(o => o.ImageId, o => o.DisplayOrder);
            try
            {
                // 2 phases update
                // Phase 1: Temporary set display order to an offset to avoid DB unique constraint
                await uow.BeginTransactionAsync(cancellationToken);
                foreach (var img in product.ProductImages) img.DisplayOrder += Temp_Order_Offset;
                await uow.SaveChangesAsync(cancellationToken);

                // Phase 2: apply the real target orders
                foreach (var img in product.ProductImages) img.DisplayOrder = orderLookup[img.Id];

                product.MarkEntityAsUpdated();
                await uow.SaveChangesAsync(cancellationToken);

                await uow.CommitTransactionAsync(cancellationToken);
                return Result.Updated;
            }
            catch (Exception ex)
            {
                await uow.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex,
                    "Failed to reorder images for product '{ProductId}': {ExceptionMsg}",
                    request.Id, ex.Message);
                return Errors.ProductErrors.UpdateImagesFailed(request.Id.ToString());
            }
        }
    }
}
