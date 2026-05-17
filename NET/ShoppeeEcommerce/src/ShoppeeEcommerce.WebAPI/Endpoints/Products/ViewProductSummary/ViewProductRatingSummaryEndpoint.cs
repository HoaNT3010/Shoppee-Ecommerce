using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Products.ViewRatingSummary;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Products.ViewRatingSummary;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Products.ViewProductSummary
{
    public class ViewProductRatingSummaryEndpoint
        : BaseEndpoint<PathGuidIdRequest, ViewProductRatingSummaryResponse?>
    {
        public ViewProductRatingSummaryEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/products/{id}/rating-summary")]
        [ApiVersion(1)]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get rating summary of a product, consists of average rating, rating count and distribution.",
            Tags = new[] { EndpointTags.Products })]
        public override async Task<ActionResult<ViewProductRatingSummaryResponse?>> HandleAsync(
            [FromRoute] PathGuidIdRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new ViewProductRatingSummaryQuery(Guid.Parse(request.Id));
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
