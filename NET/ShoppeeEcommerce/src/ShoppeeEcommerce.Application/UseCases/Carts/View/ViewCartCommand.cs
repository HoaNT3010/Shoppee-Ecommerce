using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.View;

namespace ShoppeeEcommerce.Application.UseCases.Carts.View
{
    public record ViewCartCommand()
        : IRequest<ErrorOr<ViewCartResponse>>;
}
