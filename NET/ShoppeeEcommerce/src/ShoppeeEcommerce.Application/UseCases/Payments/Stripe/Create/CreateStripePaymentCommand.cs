using ErrorOr;
using MediatR;
using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Create;

namespace ShoppeeEcommerce.Application.UseCases.Payments.Stripe.Create
{
    public record CreateStripePaymentCommand(
        Guid OrderId,
        Guid UserId)
        : IRequest<ErrorOr<CreateStripePaymentResponse>>;
}
