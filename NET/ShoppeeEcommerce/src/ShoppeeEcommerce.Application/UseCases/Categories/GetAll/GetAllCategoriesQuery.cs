using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;

namespace ShoppeeEcommerce.Application.UseCases.Categories.GetAll
{
    public record GetAllCategoriesQuery()
        : IRequest<ErrorOr<List<BaseCategoryResponse>>>;
}
