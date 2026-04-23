using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;

namespace ShoppeeEcommerce.Application.UseCases.Categories.GetById
{
    public record GetCategoryByIdQuery(
        Guid Id)
        : IRequest<ErrorOr<DetailedCategoryResponse>>;
}
