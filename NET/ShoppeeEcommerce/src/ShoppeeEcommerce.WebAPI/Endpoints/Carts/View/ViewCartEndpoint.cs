using Ardalis.ApiEndpoints;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Carts.View;
using ShoppeeEcommerce.SharedViewModels.Models.Carts.View;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Carts.View
{
    public class ViewCartEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<ViewCartResponse>
    {

        [HttpGet("api/v{version:apiVersion}/cart")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "View current shopping cart.",
            Tags = new[] { EndpointTags.Cart })]
        public override async Task<ActionResult<ViewCartResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var command = new ViewCartCommand();
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
