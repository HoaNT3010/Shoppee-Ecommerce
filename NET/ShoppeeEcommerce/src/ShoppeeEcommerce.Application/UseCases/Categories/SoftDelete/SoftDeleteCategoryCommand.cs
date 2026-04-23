using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Categories.SoftDelete
{
    public record SoftDeleteCategoryCommand(
        Guid Id)
        : IRequest<ErrorOr<Deleted>>;
}
