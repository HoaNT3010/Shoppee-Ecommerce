using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.RestoreSoftDeleted;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.RestoreSoftDeleted
{
    public class RestoreSoftDeletedProductEndpoint
        : BaseEndpoint<PathGuidIdRequest, Updated>
    {
        public RestoreSoftDeletedProductEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPost("api/v{version:apiVersion}/admin/products/{id}/restore")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Restore soft deleted product. For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<Updated>> HandleAsync(
            [FromRoute] PathGuidIdRequest request, CancellationToken cancellationToken = default)
        {
            var command = new RestoreSoftDeletedProductCommand(Guid.Parse(request.Id));
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
