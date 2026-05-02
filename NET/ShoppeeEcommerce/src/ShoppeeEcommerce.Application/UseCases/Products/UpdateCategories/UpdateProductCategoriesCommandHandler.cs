using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Common;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Products.UpdateCategories
{
    internal class UpdateProductCategoriesCommandHandler(
        IRepository<Product, Guid> repo,
        IRepository<Category, Guid> cateRepo,
        IUnitOfWork uow,
        ILogger<UpdateProductCategoriesCommandHandler> logger)
        : IRequestHandler<UpdateProductCategoriesCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(UpdateProductCategoriesCommand request, CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new ProductByIdSpec(
                    request.Id,
                    includeImages: false,
                    asTracking: true,
                    ignoreQueryFilter: true,
                    includeCategories: true,
                    includeCreator: false,
                    onlyActiveProduct: false),
                cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());
            var categories = await cateRepo.ListAsync(new CategoriesByIdsSpec(request.CategoryIds), cancellationToken);
            if (categories.Count != request.CategoryIds.Count)
                return Errors.CategoryErrors.InvalidCategoryIds();
            product.Categories.Clear();
            foreach (var cate in categories)
            {
                product.Categories.Add(cate);
            }
            product.SetUpdatedDateTime();
            try
            {
                await uow.SaveChangesAsync(cancellationToken);
                return Result.Updated;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to update categories for product '{ProductId}': {ExceptionMsg}",
                    request.Id, ex.Message);
                return Errors.ProductErrors.UpdateFailed();
            }
        }
    }
}
