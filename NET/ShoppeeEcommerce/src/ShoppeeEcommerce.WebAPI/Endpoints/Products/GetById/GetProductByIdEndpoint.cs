using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.GetById;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.GetById
{
    public class GetProductByIdEndpoint
        : BaseEndpoint<PathGuidIdRequest, BaseProductResponse>
    {
        public GetProductByIdEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/products/{id}")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "get product by product's ID. The result product will have its images and categories included.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<BaseProductResponse>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProductByIdQuery(Guid.Parse(request.Id));
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
