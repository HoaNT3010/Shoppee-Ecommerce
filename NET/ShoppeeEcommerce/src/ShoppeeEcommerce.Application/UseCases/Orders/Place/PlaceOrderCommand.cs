using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Orders.Place
{
    public record PlaceOrderCommand(
        Guid UserId)
        : IRequest<ErrorOr<Created>>;
}
