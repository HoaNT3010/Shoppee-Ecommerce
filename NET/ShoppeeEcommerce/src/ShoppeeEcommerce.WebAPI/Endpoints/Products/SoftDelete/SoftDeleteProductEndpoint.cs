using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.SoftDelete;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.SoftDelete
{
    public class SoftDeleteProductEndpoint
        : BaseEndpoint<PathGuidIdRequest, Deleted>
    {
        public SoftDeleteProductEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpDelete("api/v{version:apiVersion}/admin/products/{id}")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Soft delete product. For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<Deleted>> HandleAsync(
            [FromRoute] PathGuidIdRequest request, CancellationToken cancellationToken = default)
        {
            var command = new SoftDeleteProductCommand(Guid.Parse(request.Id));
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
