using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Products;

namespace ShoppeeEcommerce.Application.UseCases.Products.GetById
{
    public record GetProductByIdQuery(
        Guid Id)
        : IRequest<ErrorOr<BaseProductResponse>>;
}
