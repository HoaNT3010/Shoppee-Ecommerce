using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Products.AdminGetById;

namespace ShoppeeEcommerce.Application.UseCases.Products.AdminGetById
{
    public record AdminGetProductByIdQuery(
        Guid Id)
        : IRequest<ErrorOr<AdminGetProductByIdResponse>>;
}
