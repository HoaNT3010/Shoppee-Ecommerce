using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.PublishProduct
{
    internal class PublishProductCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        ILogger<PublishProductCommandHandler> logger)
        : IRequestHandler<PublishProductCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(PublishProductCommand request, CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new ProductByIdSpec(request.Id, true, true, true, true, onlyActiveProduct: false), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());
            if (product.Status == Domain.Enums.ProductStatus.Published) return Errors.ProductErrors.AlreadyPublished(request.Id.ToString());

            var errors = ValidateForPublish(product);
            if (errors.Count > 0) return errors;

            product.Status = Domain.Enums.ProductStatus.Published;

            try
            {
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Updated;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to publish product '{ProductId}': {ExceptionMsg}", request.Id, ex.Message);
                return Errors.ProductErrors.PublishProductFailed(request.Id.ToString());
            }
        }

        private static List<Error> ValidateForPublish(Product product)
        {
            var errors = new List<Error>();

            if (product.Price <= 0)
                errors.Add(Errors.ProductErrors.InvalidPrice);

            if (product.Categories.Count == 0)
                errors.Add(Errors.ProductErrors.MissingCategories);

            if (product.ProductImages.Count == 0)
                errors.Add(Errors.ProductErrors.MissingImages);
            // only check if images exist
            else if (!product.ProductImages.Any(x => x.IsMain))
                errors.Add(Errors.ProductErrors.MissingMainImage);

            return errors;
        }
    }
}
