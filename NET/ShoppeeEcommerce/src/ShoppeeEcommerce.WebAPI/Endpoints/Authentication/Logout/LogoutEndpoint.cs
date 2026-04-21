using Ardalis.ApiEndpoints;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Authentication.Logout;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.Logout;
using ShoppeeEcommerce.WebAPI.Utilities;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.Logout
{
    public class LogoutEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithRequest<LogoutRequest>
        .WithActionResult<Deleted>
    {
        [Authorize]
        [HttpPost("auth/logout")]
        public override async Task<ActionResult<Deleted>> HandleAsync(
            LogoutRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new LogoutCommand(request.RefreshToken);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
