using ErrorOr;
using MediatR;

namespace ShoppeeEcommerce.Application.UseCases.Payments.Stripe.Refund
{
    public record CreateStripeRefundCommand(
        Guid PaymentId,
        decimal Amount,
        // UserId and OrderId for future validation
        Guid OrderId,
        Guid UserId)
        : IRequest<ErrorOr<Created>>;
}
