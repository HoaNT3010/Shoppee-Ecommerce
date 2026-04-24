using Ardalis.ApiEndpoints;
using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Authentication.Logout;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Logout;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.Logout
{
    public class LogoutEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithRequest<LogoutRequest>
        .WithActionResult<Deleted>
    {
        [HttpPost("api/v{version:apiVersion}/auth/logout")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Allows application's users to sign-out out of the system.",
            Tags = new[] { EndpointTags.Authentication })]
        public override async Task<ActionResult<Deleted>> HandleAsync(
            [FromBody] LogoutRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new LogoutCommand(request.RefreshToken);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
