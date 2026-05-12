using ErrorOr;
using MediatR;
using ShoppeeEcommerce.Application.Abstractions.Payments.Stripe;

namespace ShoppeeEcommerce.Application.UseCases.Payments.Stripe.Refund
{
    internal class CreateStripeRefundCommandHandler(
        IStripePaymentService paymentService)
        : IRequestHandler<CreateStripeRefundCommand, ErrorOr<Created>>
    {
        public async Task<ErrorOr<Created>> Handle(
            CreateStripeRefundCommand request,
            CancellationToken cancellationToken)
        {
            // Future: Validate UserId in the request with UserId in the payment/order
            // To prevent random users refund payments/orders that are not them

            // Not pass the refund amount to use default payment amount as the refund amount
            var paymentResult = await paymentService.RefundAsync(request.PaymentId, null, cancellationToken);
            if (paymentResult.IsError) return paymentResult.FirstError;
            return Result.Created;
        }
    }
}
