using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.UploadImages;
using ShoppeeEcommerce.SharedViewModels.Models.Products.UploadImages;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.UploadImages
{
    public class UploadProductImagesEndpoint
        : BaseEndpoint<UploadProductImagesRequest, UploadProductImagesResponse>
    {
        public UploadProductImagesEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPost("api/v{version:apiVersion}/admin/products/{id}/images")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Upload images for product. For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        [IgnoreAntiforgeryToken]
        public override async Task<ActionResult<UploadProductImagesResponse>> HandleAsync(
            UploadProductImagesRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UploadProductImagesCommand(request.Images, Guid.Parse(request.Id!));
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
