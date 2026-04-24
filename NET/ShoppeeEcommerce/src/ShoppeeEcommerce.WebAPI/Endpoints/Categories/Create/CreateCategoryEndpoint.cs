using Ardalis.ApiEndpoints;
using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Categories.Create;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.Create;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.Create
{
    public class CreateCategoryEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithRequest<CreateCategoryRequest>
        .WithActionResult<Created>
    {
        [HttpPost("api/v{version:apiVersion}/admin/categories")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Create new category. For ADMIN users only.",
            Tags = new[] { EndpointTags.Categories })]
        public override async Task<ActionResult<Created>> HandleAsync(
            [FromBody] CreateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = HttpContext.User.GetUserId();
            var command = new CreateCategoryCommand(
                request.Name.Trim(),
                request.Description.Trim(),
                userId);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
