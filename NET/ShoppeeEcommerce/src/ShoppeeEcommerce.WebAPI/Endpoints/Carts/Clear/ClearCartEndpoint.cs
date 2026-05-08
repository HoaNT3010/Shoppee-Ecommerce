using Ardalis.ApiEndpoints;
using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Carts.Clear;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Carts.Clear
{
    public class ClearCartEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<Updated>
    {
        [HttpDelete("api/v{version:apiVersion}/cart")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Clear all products to cart.",
            Tags = new[] { EndpointTags.Cart })]
        public override async Task<ActionResult<Updated>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var command = new ClearCartCommand();
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
