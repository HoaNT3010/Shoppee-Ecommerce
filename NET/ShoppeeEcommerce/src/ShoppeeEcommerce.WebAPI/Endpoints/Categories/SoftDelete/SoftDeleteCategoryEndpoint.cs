using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Categories.SoftDelete;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.SoftDelete
{
    public class SoftDeleteCategoryEndpoint
        : BaseEndpoint<PathGuidIdRequest, Deleted>
    {
        public SoftDeleteCategoryEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpDelete("api/v{version:apiVersion}/admin/categories/{id}")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Soft delete category. For ADMIN users only.",
            Tags = new[] { EndpointTags.Categories })]
        public override async Task<ActionResult<Deleted>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new SoftDeleteCategoryCommand(Guid.Parse(request.Id));
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
