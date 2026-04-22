using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Categories.Create
{
    public record CreateCategoryCommand(
        string Name,
        string Description,
        Guid CreatorId)
        : IRequest<ErrorOr<Created>>;
}
