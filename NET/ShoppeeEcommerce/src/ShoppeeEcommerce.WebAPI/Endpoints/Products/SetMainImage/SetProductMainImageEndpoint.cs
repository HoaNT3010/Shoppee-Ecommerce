using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.SetMainImage;
using ShoppeeEcommerce.SharedViewModels.Models.Products.SetMainImage;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.SetMainImage
{
    public class SetProductMainImageEndpoint
        : BaseEndpoint<
            SetProductMainImageRequest,
            Updated>
    {
        public SetProductMainImageEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPatch("api/v{version:apiVersion}/admin/products/{id}/images/{imageId}/main")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Set main image of a product. For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<Updated>> HandleAsync(
            [FromRoute] SetProductMainImageRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new SetProductMainImageCommand(Guid.Parse(request.Id), request.ImageId);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
