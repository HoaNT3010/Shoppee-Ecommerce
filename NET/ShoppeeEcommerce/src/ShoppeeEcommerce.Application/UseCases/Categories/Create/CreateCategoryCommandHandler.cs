using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using ShoppeeEcommerce.Application.Abstractions.DataAccess;
using ShoppeeEcommerce.Application.UseCases.Categories.Common.Specifications;
using ShoppeeEcommerce.Domain.Entities.Core;
using ShoppeeEcommerce.Domain.Errors;

namespace ShoppeeEcommerce.Application.UseCases.Categories.Create
{
    internal class CreateCategoryCommandHandler(
        IRepository<Category, Guid> repo,
        ILogger<CreateCategoryCommandHandler> logger,
        IUnitOfWork uow)
        : IRequestHandler<CreateCategoryCommand, ErrorOr<Created>>
    {
        public async Task<ErrorOr<Created>> Handle(
            CreateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var isDuplicatedName = await repo.AnyAsync(
                new DuplicatedCategoryNameSpec(request.Name),
                cancellationToken);
            if (isDuplicatedName) return Errors.CategoryErrors.CategoryNameDuplicated(request.Name);
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                CreatorId = request.CreatorId
            };
            try
            {
                await repo.AddAsync(category, cancellationToken);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when trying to create new category.");
                return Errors.CategoryErrors.CreateCategoryFailed();
            }
            return Result.Created;
        }
    }
}
