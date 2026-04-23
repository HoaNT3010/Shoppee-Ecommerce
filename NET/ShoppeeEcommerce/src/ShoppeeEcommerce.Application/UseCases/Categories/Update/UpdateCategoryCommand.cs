using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Categories.Update
{
    public record UpdateCategoryCommand(
        Guid CategoryId,
        string? Name,
        string? Description)
        : IRequest<ErrorOr<Updated>>;
}
