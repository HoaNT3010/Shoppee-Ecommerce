using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.Feature;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.Feature
{
    public class FeatureProductEndpoint
        : BaseEndpoint<PathGuidIdRequest, Updated>
    {
        public FeatureProductEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPatch("api/v{version:apiVersion}/admin/products/{id}/feature")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Mark a product as featured. For ADMIN users only.",
            Tags = new[] { EndpointTags.ProductsAdmin })]
        public override async Task<ActionResult<Updated>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new FeatureProductCommand(Guid.Parse(request.Id));
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
