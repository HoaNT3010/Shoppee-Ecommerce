using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Categories.RestoreSoftDeleted
{
    internal class RestoreSoftDeletedCategoryCommandHandler(
        ILogger<RestoreSoftDeletedCategoryCommandHandler> logger,
        IRepository<Category, Guid> repo,
        IUnitOfWork uow)
        : IRequestHandler<RestoreSoftDeletedCategoryCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(
            RestoreSoftDeletedCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var category = await repo.FirstOrDefaultAsync(
                new GetCategoryByIdSpec(
                    request.Id,
                    asTracking: true,
                    ignoreQueryFilter: true),
                cancellationToken);
            if (category is null) return Errors.CategoryErrors.NotFoundWithId(request.Id.ToString());

            if (!category.IsDeleted) return Result.Updated;

            category.RestoreSoftDelete();
            try
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to restore soft deleted category with ID '{categoryId}'.", request.Id);
                return Errors.CategoryErrors.RestoreSoftDeletedCategoryFailed();
            }
            return Result.Updated;
        }
    }
}
