using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.AdminGetById;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products.AdminGetById;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.AdminGetById
{
    public class AdminGetProductByIdEndpoint
        : BaseEndpoint<
            PathGuidIdRequest,
            AdminGetProductByIdResponse>
    {
        public AdminGetProductByIdEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/admin/products/{id}")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Get product by product's ID. The result product will have its images, categories and creator included. For ADMIN users only.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<AdminGetProductByIdResponse>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new AdminGetProductByIdQuery(Guid.Parse(request.Id));
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
