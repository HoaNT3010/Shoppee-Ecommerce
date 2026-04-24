using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.AdminListCategories;
using ShoppeeEcommerce.SharedViewModels.Models.Common;

namespace ShoppeeEcommerce.Application.UseCases.Categories.AdminListCategories
{
    internal class AdminListCategoriesQueryHandler(
        IRepository<Category, Guid> repo)
        : IRequestHandler<
            AdminListCategoriesQuery,
            ErrorOr<PagedList<AdminListCategoryResponse>>>
    {
        public async Task<ErrorOr<PagedList<AdminListCategoryResponse>>> Handle(
            AdminListCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            var filterSpec = new AdminListCategoriesFilterSpecification(request);
            var pagingSpec = new AdminListCategoriesPagingSpecification(request);

            var totalItems = await repo.CountAsync(filterSpec, cancellationToken);
            var items = await repo.ListAsync(pagingSpec, cancellationToken);

            // Safely use null-forgiving operator for page index and page size
            // Since they will be assigned with default values
            return PagedList.Create(items, totalItems, request.PageIndex!.Value, request.PageSize!.Value);
        }
    }
}
