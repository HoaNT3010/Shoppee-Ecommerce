using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Categories.SoftDelete
{
    internal class SoftDeleteCategoryCommandHandler(
        IRepository<Category, Guid> repo,
        IUnitOfWork uow,
        ILogger<SoftDeleteCategoryCommandHandler> logger)
        : IRequestHandler<SoftDeleteCategoryCommand, ErrorOr<Deleted>>
    {
        public async Task<ErrorOr<Deleted>> Handle(
            SoftDeleteCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var category = await repo.FirstOrDefaultAsync(
                new GetCategoryByIdSpec(
                    request.Id,
                    asTracking: true,
                    ignoreQueryFilter: true),
                cancellationToken);
            if (category is null) return Errors.CategoryErrors.NotFoundWithId(request.Id.ToString());

            if (category.IsDeleted) return Result.Deleted;

            category.SoftDelete();
            try
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to soft delete category with ID '{categoryId}'.", request.Id);
                return Errors.CategoryErrors.SoftDeleteCategoryFailed();
            }
            return Result.Deleted;
        }
    }
}
