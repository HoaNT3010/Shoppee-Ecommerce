using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;
using ShoppeeEcommerce.SharedViewModels.Models.Products;
using ShoppeeEcommerce.SharedViewModels.Models.Products.AdminGetById;
using ShoppeeEcommerce.SharedViewModels.Models.Users;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminGetById
{
    internal class AdminGetProductByIdQueryHandler(
        IRepository<Product, Guid> repo)
        : IRequestHandler<AdminGetProductByIdQuery, ErrorOr<AdminGetProductByIdResponse>>
    {
        public async Task<ErrorOr<AdminGetProductByIdResponse>> Handle(
            AdminGetProductByIdQuery request,
            CancellationToken cancellationToken)
        {
            var product = await repo.FirstOrDefaultAsync(
                new AdminProductDetailsSpec(request.Id), cancellationToken);
            if (product is null) return Errors.ProductErrors.ProductNotFoundWithId(request.Id.ToString());

            var creator = product.Creator == null ? null : new BaseCreatorResponse(
                product.Creator.Id,
                product.Creator.UserName!,
                product.Creator.Email!,
                product.Creator.FirstName,
                product.Creator.LastName);

            return new AdminGetProductByIdResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                Images = product.OrderedImages.Select(pi => new BaseProductImageResponse(pi.Id, pi.Url, pi.IsMain, pi.DisplayOrder, pi.AltText)).ToList(),
                Categories = product.Categories.Select(c => new ShortCategoryResponse(c.Id, c.Name)).ToList(),
                Creator = creator,
                Status = product.Status.ToString(),
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
                IsDeleted = product.IsDeleted,
                DeletedDate = product.DeletedDate,
            };
        }
    }
}
