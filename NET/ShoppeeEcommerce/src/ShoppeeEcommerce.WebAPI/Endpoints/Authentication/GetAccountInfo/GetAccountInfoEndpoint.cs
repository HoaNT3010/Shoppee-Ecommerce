using Ardalis.ApiEndpoints;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Authentication.GetAccountInfo;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.GetAccountInfo;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.GetAccountInfo
{
    public class GetAccountInfoEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<GetAccountInfoResponse>
    {
        [HttpGet("api/v{version:apiVersion}/auth/info")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
        [SwaggerOperation(
            Summary = "Get information of user's account. For AUTHENTICATED users only",
            Tags = new[] { EndpointTags.Authentication })]
        public override async Task<ActionResult<GetAccountInfoResponse>> HandleAsync(
            CancellationToken cancellationToken = default)
        {
            var userId = HttpContext.User.GetUserId();
            var query = new GetAccountInfoQuery(userId);
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
