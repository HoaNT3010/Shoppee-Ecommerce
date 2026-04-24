using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Categories.HardDelete
{
    internal class HardDeleteCategoryCommandHandler(
        IRepository<Category, Guid> repo,
        IUnitOfWork uow,
        ILogger<HardDeleteCategoryCommandHandler> logger)
        : IRequestHandler<HardDeleteCategoryCommand, ErrorOr<Deleted>>
    {
        public async Task<ErrorOr<Deleted>> Handle(
            HardDeleteCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var category = await repo.FirstOrDefaultAsync(
                new GetCategoryByIdSpec(
                    request.Id,
                    asTracking: true,
                    ignoreQueryFilter: true),
                cancellationToken);
            if (category is null) return Errors.CategoryErrors.NotFoundWithId(request.Id.ToString());
            try
            {
                // NOT TESTED: Category with product(s)
                // TO DO: Re-test this when Create product feature complete
                // (Can assign category to product).
                repo.Delete(category);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to hard delete category with ID '{categoryId}'.", request.Id);
                return Errors.CategoryErrors.HardDeleteCategoryFailed();
            }
            return Result.Deleted;
        }
    }
}
