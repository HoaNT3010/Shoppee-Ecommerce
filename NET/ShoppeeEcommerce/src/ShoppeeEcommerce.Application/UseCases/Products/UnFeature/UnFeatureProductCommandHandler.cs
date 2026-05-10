using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Common;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.UnFeature
{
    internal class UnFeatureProductCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        ILogger<UnFeatureProductCommandHandler> logger)
        : IRequestHandler<UnFeatureProductCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(
            UnFeatureProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new GetBaseProductByIdSpec(request.Id), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());
            if (!product.IsFeatured) return Result.Updated;

            product.IsFeatured = false;
            product.SetUpdatedDateTime();
            try
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to un-feature the product with ID '{productId}': {exceptionMsg}.", request.Id, ex.Message);
                return Errors.ProductErrors.UnFeatureFailed();
            }
            return Result.Updated;
        }
    }
}
