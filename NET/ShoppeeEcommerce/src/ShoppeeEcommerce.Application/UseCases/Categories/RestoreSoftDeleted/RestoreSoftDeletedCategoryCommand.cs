using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Categories.RestoreSoftDeleted
{
    public record RestoreSoftDeletedCategoryCommand(
        Guid Id)
        : IRequest<ErrorOr<Updated>>;
}
