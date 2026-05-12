using Ardalis.ApiEndpoints;
using Asp.Versioning;
using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.Abstractions.Payments.Stripe;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Payments.Stripe.Webhook
{
    public class StripePaymentWebhookEndpoint(
        IStripePaymentService stripeService)
        : EndpointBaseAsync
            .WithoutRequest
            .WithActionResult
    {
        [HttpPost("api/v{version:apiVersion}/payments/stripe/webhook")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "For handling Stripe webhook responses. DO NOT CALL.",
            Tags = new[] { EndpointTags.Webhooks })]
        public async override Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
        {
            // Extract the raw body for signature verification
            var json = await new StreamReader(Request.Body).ReadToEndAsync(cancellationToken);
            // Grab the signature from headers
            var signature = Request.Headers["Stripe-Signature"].ToString();

            var result = await stripeService.HandleWebhookAsync(json, signature, cancellationToken);

            // Map ErrorOr result to ActionResult
            return result.Match(
                payment => Ok(), // Success: Return 200
                errors => MapErrorsToResponse(errors));
        }

        private ActionResult MapErrorsToResponse(List<Error> errors)
        {
            var firstError = errors[0];

            return firstError.Code switch
            {
                // If we don't support the event, we return 200 OK. 
                // This tells Stripe "Message received, but we don't need to do anything."
                "Payment.Stripe.EventNotSupported" => Ok(),

                // Signature errors mean the request didn't actually come from Stripe
                "Payment.Stripe.InvalidWebhookSignature" => BadRequest(firstError.Description),

                // Use 404 if the payment intent ID in Stripe doesn't exist in our DB
                "Payment.Stripe.PaymentNotFoundWithIntentId" => NotFound(firstError.Description),

                // For unexpected processing errors, return a 500 so Stripe retries later
                _ => Problem(detail: firstError.Description, statusCode: 500)
            };
        }
    }
}
