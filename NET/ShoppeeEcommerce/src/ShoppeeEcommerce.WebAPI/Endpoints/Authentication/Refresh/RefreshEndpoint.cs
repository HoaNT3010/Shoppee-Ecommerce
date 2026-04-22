using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Authentication.Refresh;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Refresh;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.Refresh
{
    public class RefreshEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithRequest<RefreshRequest>
        .WithActionResult<RefreshResponse>
    {
        [AllowAnonymous]
        [HttpPost("api/auth/refresh")]
        [SwaggerOperation(
            Summary = "Allows application's users refresh/rotate their authentication credentials.",
            Tags = new[] { EndpointTags.Authentication })]
        public override async Task<ActionResult<RefreshResponse>> HandleAsync(
            [FromBody] RefreshRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new RefreshCommand(request.RefreshToken);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
