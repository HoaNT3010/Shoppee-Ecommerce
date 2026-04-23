using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;
using ShoppeeEcommerce.SharedViewModels.Models.Categories;

namespace ShoppeeEcommerce.Application.UseCases.Categories.GetById
{
    internal class GetCategoryByIdQueryHandler(
        IRepository<Category, Guid> repo)
        : IRequestHandler<GetCategoryByIdQuery, ErrorOr<DetailedCategoryResponse>>
    {
        public async Task<ErrorOr<DetailedCategoryResponse>> Handle(
            GetCategoryByIdQuery request,
            CancellationToken cancellationToken)
        {
            var result = await repo.FirstOrDefaultAsync(
                new GetCategoryWithCreatorByIdSpec(
                    request.Id,
                    asTracking: false,
                    ignoreQueryFilter: true),
                cancellationToken);
            return result is null
                ? Errors.CategoryErrors.NotFoundWithId(request.Id.ToString())
                : result;
        }
    }
}
