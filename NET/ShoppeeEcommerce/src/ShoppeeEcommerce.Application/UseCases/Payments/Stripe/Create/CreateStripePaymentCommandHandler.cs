using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.Payments.Stripe;
using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Create;

namespace ShoppeeEcommerce.Application.UseCases.Payments.Stripe.Create
{
    internal class CreateStripePaymentCommandHandler(
        IStripePaymentService paymentService)
        : IRequestHandler<CreateStripePaymentCommand, ErrorOr<CreateStripePaymentResponse>>
    {
        public async Task<ErrorOr<CreateStripePaymentResponse>> Handle(CreateStripePaymentCommand request, CancellationToken cancellationToken)
        {
            var result = await paymentService.CreatePaymentIntentAsync(request.OrderId, request.UserId, cancellationToken);
            if (result.IsError) return result.FirstError;
            return new CreateStripePaymentResponse
            {
                Amount = result.Value.Amount,
                Currency = result.Value.Currency.ToLowerInvariant(),
                StripePaymentIntentId = result.Value.StripePaymentIntentId,
                StripeClientSecret = result.Value.StripeClientSecret!
            };
        }
    }
}
