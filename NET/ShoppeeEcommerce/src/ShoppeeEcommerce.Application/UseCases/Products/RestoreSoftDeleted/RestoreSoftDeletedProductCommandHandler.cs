using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.SoftDelete;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.RestoreSoftDeleted
{
    internal class RestoreSoftDeletedProductCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        ILogger<RestoreSoftDeletedProductCommandHandler> logger)
        : IRequestHandler<RestoreSoftDeletedProductCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(
            RestoreSoftDeletedProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new GetBaseProductByIdSpec(request.Id), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());
            if (!product.IsDeleted) return Result.Updated;

            product.RestoreSoftDelete();
            try
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to restore soft deleted product with ID '{productId}': {exceptionMsg}.", request.Id, ex.Message);
                return Errors.ProductErrors.RestoreSoftDeletedFailed();
            }
            return Result.Updated;
        }
    }
}
