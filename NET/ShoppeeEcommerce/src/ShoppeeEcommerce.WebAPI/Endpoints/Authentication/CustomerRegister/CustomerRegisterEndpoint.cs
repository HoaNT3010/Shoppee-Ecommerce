using Ardalis.ApiEndpoints;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Authentication.CustomerRegister;
using ShoppeeEcommerce.SharedViewModels.Models.Authentication.CustomerRegister;
using ShoppeeEcommerce.WebAPI.Utilities;

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
        public override async Task<ActionResult<Created>> HandleAsync(
            CustomerRegisterRequest request,
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
