using Ardalis.ApiEndpoints;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Authentication.CustomerRegister;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.CustomerRegister;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Authentication.CustomerRegister
{
    public class CustomerRegisterEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithRequest<CustomerRegisterRequest>
        .WithActionResult<Created>
    {
        [HttpPost("customers/register")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Allows new user to register as application's customer.",
            Tags = new[] { EndpointTags.Authentication })]
        public override async Task<ActionResult<Created>> HandleAsync(
            [FromBody] CustomerRegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CustomerRegisterCommand(
                request.UserName,
                request.Email,
                request.Password,
                request.ConfirmPassword,
                request.FirstName,
                request.LastName);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
