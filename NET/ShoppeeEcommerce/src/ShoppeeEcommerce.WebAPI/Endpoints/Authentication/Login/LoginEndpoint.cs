using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Authentication.Login;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Login;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.Login
{
    public class LoginEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithRequest<LoginRequest>
        .WithActionResult<LoginResponse>
    {
        [HttpPost("auth/login")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Allows application's users to sign-in into the system.",
            Tags = new[] { EndpointTags.Authentication })]
        public override async Task<ActionResult<LoginResponse>> HandleAsync(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new LoginCommand(request.Email, request.Password);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
