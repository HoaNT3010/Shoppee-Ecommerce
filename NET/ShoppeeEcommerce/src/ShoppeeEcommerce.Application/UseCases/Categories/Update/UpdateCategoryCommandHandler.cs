using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Domain.Common;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Categories.Update
{
    internal class UpdateCategoryCommandHandler(
        IUnitOfWork uow,
        IRepository<Category, Guid> repo,
        ILogger<UpdateCategoryCommandHandler> logger)
        : IRequestHandler<UpdateCategoryCommand, ErrorOr<Updated>>
    {
        public async Task<ErrorOr<Updated>> Handle(
            UpdateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            // If both properties are empty, return
            if (string.IsNullOrWhiteSpace(request.Name) && string.IsNullOrWhiteSpace(request.Description)) return Result.Updated;

            var category = await repo.FirstOrDefaultAsync(new GetCategoryByIdSpec(request.CategoryId));
            if (category is null) return Errors.CategoryErrors.NotFoundWithId(request.CategoryId.ToString());

            if (request.Name is not null && request.Name != category.Name)
            {
                // Check for name duplication
                var isDuplicatedName = await repo.AnyAsync(
                    new DuplicatedCategoryNameSpec(request.Name),
                    cancellationToken);
                if (isDuplicatedName) return Errors.CategoryErrors.CategoryNameDuplicated(request.Name);
                category.Name = request.Name;
            }
            if (request.Description is not null) category.Description = request.Description;
            category.SetUpdatedDateTime();

            try
            {
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to update category.");
                return Errors.CategoryErrors.UpdateCategoryFailed();
            }

            return Result.Updated;
        }
    }
}
