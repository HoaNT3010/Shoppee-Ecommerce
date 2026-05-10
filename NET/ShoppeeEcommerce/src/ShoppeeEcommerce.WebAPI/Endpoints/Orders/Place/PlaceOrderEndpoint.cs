using Ardalis.ApiEndpoints;
using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Orders.Place;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Orders.Place
{
    public class PlaceOrderEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<Created>
    {
        [HttpPost("api/v{version:apiVersion}/orders/place")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
        [SwaggerOperation(
            Summary = "Place new order based on products in shopping cart. For AUTHENTICATED users only.",
            Tags = new[] { EndpointTags.Orders })]
        public override async Task<ActionResult<Created>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var userId = HttpContext.User.GetUserId();
            var command = new PlaceOrderCommand(userId);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
