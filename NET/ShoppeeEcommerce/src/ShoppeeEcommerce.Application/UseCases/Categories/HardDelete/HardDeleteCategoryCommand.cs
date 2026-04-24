using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Categories.HardDelete
{
    public record HardDeleteCategoryCommand(
        Guid Id)
        : IRequest<ErrorOr<Deleted>>;
}
