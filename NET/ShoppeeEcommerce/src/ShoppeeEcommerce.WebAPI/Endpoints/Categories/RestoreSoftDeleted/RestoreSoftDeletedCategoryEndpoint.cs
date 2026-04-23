using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Categories.RestoreSoftDeleted;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Categories.RestoreSoftDeleted
{
    public class RestoreSoftDeletedCategoryEndpoint
        : BaseEndpoint<PathGuidIdRequest, Updated>
    {
        public RestoreSoftDeletedCategoryEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPost("api/categories/{id}/restore")]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Restore a soft deleted category. Only allow Admin user.",
            Tags = new[] { EndpointTags.Categories })]
        public override async Task<ActionResult<Updated>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new RestoreSoftDeletedCategoryCommand(Guid.Parse(request.Id));
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
