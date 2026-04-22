using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;

namespace ShoppeeEcommerce.Application.UseCases.Categories.GetAll
{
    internal class GetAllCategoriesQueryHandler(
        IRepository<Category, Guid> repo)
        : IRequestHandler<GetAllCategoriesQuery, ErrorOr<List<BaseCategoryResponse>>>
    {
        public async Task<ErrorOr<List<BaseCategoryResponse>>> Handle(
            GetAllCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            return await repo.ListAsync(
                new GetAllCategoriesSpec(true),
                cancellationToken);
        }
    }
}
