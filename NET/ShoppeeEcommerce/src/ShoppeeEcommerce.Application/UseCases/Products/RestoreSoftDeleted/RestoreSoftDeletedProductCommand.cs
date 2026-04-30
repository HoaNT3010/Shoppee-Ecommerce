using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Products.RestoreSoftDeleted
{
    public record RestoreSoftDeletedProductCommand(
        Guid Id)
        : IRequest<ErrorOr<Updated>>;
}
