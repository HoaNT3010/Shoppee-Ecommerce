using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.GetNewest;
using ShoppeeEcommerce.SharedViewModels.Models.Products;
using ShoppeeEcommerce.SharedViewModels.Models.Products.GetNewest;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.GetNewest
{
    public class GetNewestProductsEndpoint
        : BaseEndpoint<GetNewestProductsRequest, List<BaseProductSummaryResponse>>
    {
        public GetNewestProductsEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/products/newest")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Fetch the newest products. If no product count specified, 8 products will be fetched by default.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<List<BaseProductSummaryResponse>>> HandleAsync(
            [FromQuery] GetNewestProductsRequest request,
            CancellationToken cancellationToken = default)
        {
            // Default 8 products
            var query = new GetNewestProductsQuery(request.Count ?? 8);
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
