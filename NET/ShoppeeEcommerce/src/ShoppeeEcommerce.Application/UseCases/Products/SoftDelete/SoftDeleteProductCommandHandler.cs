using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.SoftDelete
{
    internal class SoftDeleteProductCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        ILogger<SoftDeleteProductCommandHandler> logger)
        : IRequestHandler<SoftDeleteProductCommand, ErrorOr<Deleted>>
    {
        public async Task<ErrorOr<Deleted>> Handle(
            SoftDeleteProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new GetBaseProductByIdSpec(request.Id), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());
            if (product.IsDeleted) return Result.Deleted;

            product.SoftDelete();
            try
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to soft delete product with ID '{productId}': {exceptionMsg}.", request.Id, ex.Message);
                return Errors.ProductErrors.SoftDeleteFailed();
            }
            return Result.Deleted;
        }
    }
}
