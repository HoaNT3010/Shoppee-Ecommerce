using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Common;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.SetMainImage
{
    internal class SetProductMainImageCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        ILogger<SetProductMainImageCommandHandler> logger)
        : IRequestHandler<SetProductMainImageCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(
            SetProductMainImageCommand request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new ProductByIdSpec(request.ProductId, true, true, true, onlyActiveProduct: false),
                cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.ProductId.ToString());
            if (product.ProductImages.Count == 0) return Errors.ProductErrors.EmptyProductImages();

            var target = product.ProductImages.FirstOrDefault(x => x.Id == request.ImageId);
            if (target is null) return Errors.ProductErrors.HasNoSpecificImage(request.ImageId, request.ProductId.ToString());
            if (target.IsMain) return Result.Updated;

            try
            {
                // 2 steps update to avoid violating unique index
                // Could use ExecuteUpdate for a Single Atomic SQL Statement
                // But required exposing DbSet or modify repository

                // Reset the main image first
                var currentMain = product.MainImage;
                if (currentMain is not null)
                {
                    currentMain.IsMain = false;
                    await uow.SaveChangesAsync(cancellationToken);
                }
                // Loop the list and assign new main
                foreach (var image in product.ProductImages)
                {
                    image.IsMain = image.Id == request.ImageId;
                }
                product.SetUpdatedDateTime();
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Updated;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to update main image of product '{ProductId}: {ExceptionMsg}'", request.ProductId.ToString(), ex.Message);
                return Errors.ProductErrors.UpdateMainImageFailed(request.ProductId.ToString());
            }
        }
    }
}
