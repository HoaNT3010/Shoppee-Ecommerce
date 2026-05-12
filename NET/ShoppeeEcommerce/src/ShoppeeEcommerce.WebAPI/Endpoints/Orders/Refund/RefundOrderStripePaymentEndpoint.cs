using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Payments.Stripe.Refund;
using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Refund;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Orders.Refund
{
    public class RefundOrderStripePaymentEndpoint
        : BaseEndpoint<RefundStripePaymentRequest, Created>
    {
        public RefundOrderStripePaymentEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPost("api/v{version:apiVersion}/orders/{orderId}/payment/stripe/{paymentId}/refund")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
        [SwaggerOperation(
            Summary = "Create a Stripe payment refund request for the order. For AUTHENTICATED users only.",
            Tags = new[] { EndpointTags.Orders })]
        public override async Task<ActionResult<Created>> HandleAsync(
            RefundStripePaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = HttpContext.User.GetUserId();
            var command = new CreateStripeRefundCommand(
                Guid.Parse(request.PaymentId),
                request.Body.Amount,
                Guid.Parse(request.OrderId),
                userId);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
