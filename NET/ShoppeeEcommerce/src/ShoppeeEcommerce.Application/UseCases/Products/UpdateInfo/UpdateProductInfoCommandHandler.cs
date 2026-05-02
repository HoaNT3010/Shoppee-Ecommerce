using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Common;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.UpdateInfo
{
    internal class UpdateProductInfoCommandHandler(
        IRepository<Product, Guid> repo,
        IUnitOfWork uow,
        ILogger<UpdateProductInfoCommandHandler> logger)
        : IRequestHandler<UpdateProductInfoCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(
            UpdateProductInfoCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name) &&
                string.IsNullOrWhiteSpace(request.Description) &&
                !request.Price.HasValue &&
                string.IsNullOrWhiteSpace(request.SKU)) return Result.Updated;

            var product = await repo.FirstOrDefaultAsync(
                new GetBaseProductByIdSpec(request.Id), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());

            if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != product.Name)
            {
                // Check for name duplication
                var isDuplicatedName = await repo.AnyAsync(new ProductWithNameSpec(request.Name, false, false), cancellationToken);
                if (isDuplicatedName) return Errors.ProductErrors.ProductNameExisted(request.Name);
                product.Name = request.Name;
            }
            if (request.Description is not null) product.Description = request.Description;
            if (request.Price.HasValue && request.Price != product.Price) product.Price = request.Price.Value;
            if (!string.IsNullOrWhiteSpace(request.SKU) && request.SKU != product.SKU)
            {
                // Check for name duplication
                var isDuplicatedSku = await repo.AnyAsync(new ProductWithSKUSpec(request.SKU, false, false), cancellationToken);
                if (isDuplicatedSku) return Errors.ProductErrors.ProductSKUExisted(request.SKU);
                product.SKU = request.SKU;
            }
            product.SetUpdatedDateTime();
            try
            {
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Updated;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to update product with ID '{ProductId}': {exMsg}.", request.Id.ToString(), ex.Message);
                return Errors.ProductErrors.UpdateFailed();
            }
        }
    }
}
