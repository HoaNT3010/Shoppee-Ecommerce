using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Payments.Stripe.Create;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Payments.Stripe.Create;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Orders.CreatePayment
{
    public class CreateOrderStripePaymentEndpoint
        : BaseEndpoint<PathGuidIdRequest, CreateStripePaymentResponse>
    {
        public CreateOrderStripePaymentEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPost("api/v{version:apiVersion}/orders/{id}/payment/stripe")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
        [SwaggerOperation(
            Summary = "Create a Stripe payment request for the order. For AUTHENTICATED users only.",
            Tags = new[] { EndpointTags.Orders })]
        public override async Task<ActionResult<CreateStripePaymentResponse>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = HttpContext.User.GetUserId();
            var command = new CreateStripePaymentCommand(Guid.Parse(request.Id), userId);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
