using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.UpdateInfo;
using ShoppeeEcommerce.SharedViewModels.Models.Products.UpdateInfo;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.UpdateInfo
{
    public class UpdateProductInfoEndpoint
        : BaseEndpoint<UpdateProductInfoRequest, Updated>
    {
        public UpdateProductInfoEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPatch("api/v{version:apiVersion}/admin/products/{id}")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Update product's basic info (name, description, price, sku). For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<Updated>> HandleAsync(
            UpdateProductInfoRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Info is null) return new NoContentResult();
            var command = new UpdateProductInfoCommand(
                Guid.Parse(request.Id!),
                request.Info.Name,
                request.Info.Description,
                request.Info.Price,
                request.Info.SKU);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
