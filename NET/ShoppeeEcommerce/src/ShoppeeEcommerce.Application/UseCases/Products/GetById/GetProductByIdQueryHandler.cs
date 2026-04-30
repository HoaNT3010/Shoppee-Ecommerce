using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Products.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.GetById
{
    internal class GetProductByIdQueryHandler(
        IRepository<Product, Guid> repo)
        : IRequestHandler<GetProductByIdQuery, ErrorOr<BaseProductResponse>>
    {
        public async Task<ErrorOr<BaseProductResponse>> Handle(
            GetProductByIdQuery request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new ProductDetailsSpec(request.Id),
                cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());
            return new BaseProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                Images = product.OrderedImages.Select(pi => new BaseProductImageResponse(pi.Id, pi.Url, pi.IsMain, pi.DisplayOrder, pi.AltText)).ToList(),
                Categories = product.Categories.Select(c => new ShortCategoryResponse(c.Id, c.Name)).ToList()
            };
        }
    }
}
