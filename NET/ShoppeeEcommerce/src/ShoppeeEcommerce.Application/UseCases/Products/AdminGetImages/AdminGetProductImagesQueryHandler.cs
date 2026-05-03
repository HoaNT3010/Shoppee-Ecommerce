using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminGetImages
{
    internal class AdminGetProductImagesQueryHandler(
        IRepository<Product, Guid> repo)
        : IRequestHandler<AdminGetProductImagesQuery, ErrorOr<List<BaseProductImageResponse>>>
    {
        public async Task<ErrorOr<List<BaseProductImageResponse>>> Handle(
            AdminGetProductImagesQuery request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(new AdminGetProductImagesSpec(request.Id), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());
            return product.ProductImages.Select(i => new BaseProductImageResponse(
                i.Id,
                i.Url,
                i.IsMain,
                i.DisplayOrder,
                i.AltText)).ToList();
        }
    }
}
