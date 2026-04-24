using Ardalis.ApiEndpoints;
using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Categories.Update;
using ShoppeeEcommerce.SharedViewModels.Models.Categories.Update;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.Update
{
    public class UpdateCategoryEndpoint(
        ISender sender)
        : EndpointBaseAsync
        .WithRequest<UpdateCategoryRequest>
        .WithActionResult<Updated>
    {
        [HttpPatch("api/v{version:apiVersion}/admin/categories")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Update category. The information can be updated are Name and Description, which both are optional. For ADMIN users only.",
            Tags = new[] { EndpointTags.Categories })]
        public override async Task<ActionResult<Updated>> HandleAsync(
            UpdateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateCategoryCommand(
                Guid.Parse(request.CategoryId),
                !string.IsNullOrWhiteSpace(request.Name) ? request.Name.Trim() : null,
                !string.IsNullOrWhiteSpace(request.Description) ? request.Description.Trim() : null);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
