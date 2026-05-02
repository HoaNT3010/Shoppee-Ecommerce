using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.Abstractions.Storage;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.HardDelete
{
    internal class HardDeleteProductCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        ILogger<HardDeleteProductCommandHandler> logger,
        IFileService fs)
        : IRequestHandler<HardDeleteProductCommand, ErrorOr<Deleted>>
    {
        public async Task<ErrorOr<Deleted>> Handle(
            HardDeleteProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new ProductByIdSpec(request.Id,
                includeImages: true,
                asTracking: true,
                ignoreQueryFilter: true,
                onlyActiveProduct: false,
                asSplitQuery: true), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());
            try
            {
                repo.Delete(product);
                // TO DO: Move delete images on cloud storage process
                // to background services
                await fs.DeleteManyAsync(product.ProductImages.Select(x => x.PublicId), cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Deleted;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to hard delete product with ID '{productId}': {exceptionMsg}.", request.Id, ex.Message);
                return Errors.ProductErrors.HardDeleteFailed();
            }
        }
    }
}
