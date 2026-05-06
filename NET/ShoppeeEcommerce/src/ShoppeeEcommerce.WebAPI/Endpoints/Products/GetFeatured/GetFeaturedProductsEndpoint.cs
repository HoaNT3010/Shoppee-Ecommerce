using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.GetFeatured;
using ShoppeeEcommerce.SharedViewModels.Models.Products;
using ShoppeeEcommerce.SharedViewModels.Models.Products.GetFeatured;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.GetFeatured
{
    public class GetFeaturedProductsEndpoint
        : BaseEndpoint<GetFeaturedProductsRequest, List<BaseProductSummaryResponse>>
    {
        public GetFeaturedProductsEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/products/featured")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Fetch the featured products. If no product count specified, 8 products will be fetched by default.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<List<BaseProductSummaryResponse>>> HandleAsync(
            [FromQuery] GetFeaturedProductsRequest request,
            CancellationToken cancellationToken = default)
        {
            // Default 8 products
            var query = new GetFeaturedProductsQuery(request.Count ?? 8);
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
