using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.ReorderImages;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ReorderImages;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.ReorderImages
{
    public class ReorderProductImagesEndpoint
        : BaseEndpoint<ReorderProductImagesRequest, Updated>
    {
        public ReorderProductImagesEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPatch("api/v{version:apiVersion}/admin/products/{id}/images/reorder")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Adjust display order of product images. For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<Updated>> HandleAsync(
            ReorderProductImagesRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new ReorderProductImagesCommand(Guid.Parse(request.Id!),
                request.Orders.Orders);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
