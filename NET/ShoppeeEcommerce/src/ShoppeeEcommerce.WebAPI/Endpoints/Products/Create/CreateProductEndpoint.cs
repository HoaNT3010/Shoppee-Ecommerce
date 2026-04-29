using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.Create;
using ShoppeeEcommerce.SharedViewModels.Models.Products.Create;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.Create
{
    public class CreateProductEndpoint
        : BaseEndpoint<CreateProductRequest, CreateProductResponse>
    {
        public CreateProductEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpPost("api/v{version:apiVersion}/admin/products")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Create new product. For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        public async override Task<ActionResult<CreateProductResponse>> HandleAsync(
            CreateProductRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = HttpContext.User.GetUserId();
            var command = new CreateProductCommand(
                request.Name.Trim(),
                request.Description.Trim(),
                request.Price,
                request.SKU.Trim(),
                request.CategoryIds.Select(Guid.Parse).ToList(),
                userId);
            var result = await sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
