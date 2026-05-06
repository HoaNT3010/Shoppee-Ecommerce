using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.UnFeature;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.UnFeature
{
    public class UnFeatureProductEndpoint
        : BaseEndpoint<PathGuidIdRequest, Updated>
    {
        public UnFeatureProductEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPatch("api/v{version:apiVersion}/admin/products/{id}/un-feature")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Mark a product as not featured. For ADMIN users only.",
            Tags = new[] { EndpointTags.ProductsAdmin })]
        public override async Task<ActionResult<Updated>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UnFeatureProductCommand(Guid.Parse(request.Id));
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
