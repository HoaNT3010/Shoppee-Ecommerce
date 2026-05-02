using Asp.Versioning;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.UpdateCategories;
using ShoppeeEcommerce.SharedViewModels.Models.Products.UpdateCategories;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.UpdateCategories
{
    public class UpdateProductCategoriesEndpoint
        : BaseEndpoint<UpdateProductCategoriesRequest, Updated>
    {
        public UpdateProductCategoriesEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPatch("api/v{version:apiVersion}/admin/products/{id}/categories")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Update product's categories. For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<Updated>> HandleAsync(UpdateProductCategoriesRequest request, CancellationToken cancellationToken = default)
        {
            var command = new UpdateProductCategoriesCommand(Guid.Parse(request.Id!),
                request.Categories!.CategoryIds.Select(Guid.Parse).ToList());
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
