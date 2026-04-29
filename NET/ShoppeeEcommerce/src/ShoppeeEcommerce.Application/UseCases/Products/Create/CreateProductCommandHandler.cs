using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Products.Create;

namespace ShoppeeEcommerce.Application.UseCases.Products.Create
{
    internal class CreateProductCommandHandler(
        IRepository<Product, Guid> repo,
        IRepository<Category, Guid> cateRepo,
        IUnitOfWork uow,
        ILogger<CreateProductCommandHandler> logger)
        : IRequestHandler<CreateProductCommand, ErrorOr<CreateProductResponse>>
    {
        public async Task<ErrorOr<CreateProductResponse>> Handle(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            // Check for duplicated name & SKU
            if (await repo.AnyAsync(new ProductWithNameSpec(request.Name, false, false)))
                return Errors.ProductErrors.ProductNameExisted(request.Name);
            if (await repo.AnyAsync(new ProductWithSKUSpec(request.SKU, false, false)))
                return Errors.ProductErrors.ProductSKUExisted(request.SKU);
            // Check categories
            var categories = await cateRepo.ListAsync(new CategoriesByIdsSpec(request.CategoryIds));
            if (categories.Count != request.CategoryIds.Count)
                return Errors.CategoryErrors.InvalidCategoryIds();

            var product = new Product()
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                SKU = request.SKU,
                Categories = categories,
                Status = Domain.Enums.ProductStatus.Draft,
                CreatorId = request.UserId
            };

            try
            {
                await repo.AddAsync(product);
                await uow.SaveChangesAsync(cancellationToken);
                return new CreateProductResponse(product.Id,
                    product.Name,
                    product.Description,
                    product.Price,
                    product.SKU,
                    product.Status.ToString(),
                    categories.Select(c => c.Name).ToList(),
                    product.CreatedDate);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred when trying to add new product: {ExceptionMessage}", ex.Message);
                return Errors.ProductErrors.CreateProductFailed();
            }
        }
    }
}
