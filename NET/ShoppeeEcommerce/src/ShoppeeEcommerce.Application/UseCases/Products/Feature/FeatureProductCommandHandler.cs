using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Common;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.Feature
{
    internal class FeatureProductCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        ILogger<FeatureProductCommandHandler> logger)
        : IRequestHandler<FeatureProductCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(
            FeatureProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new GetBaseProductByIdSpec(request.Id), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());
            if (product.IsFeatured) return Result.Updated;

            product.IsFeatured = true;
            product.SetUpdatedDateTime();
            try
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to feature the product with ID '{productId}': {exceptionMsg}.", request.Id, ex.Message);
                return Errors.ProductErrors.FeatureFailed();
            }
            return Result.Updated;
        }
    }
}
