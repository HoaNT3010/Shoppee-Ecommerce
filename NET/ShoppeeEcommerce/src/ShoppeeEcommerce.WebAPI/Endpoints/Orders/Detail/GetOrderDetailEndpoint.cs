using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Orders.Detail;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.Detail;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Orders.Detail
{
    public class GetOrderDetailEndpoint
        : BaseEndpoint<PathGuidIdRequest, OrderDetailResponse>
    {
        public GetOrderDetailEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/orders/{id}")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
        [SwaggerOperation(
            Summary = "Get a user's order with full detail. For AUTHENTICATED users only.",
            Tags = new[] { EndpointTags.Orders })]
        public override async Task<ActionResult<OrderDetailResponse>> HandleAsync(
            [FromRoute] PathGuidIdRequest request, CancellationToken cancellationToken = default)
        {
            var query = new GetOrderDetailQuery(Guid.Parse(request.Id));
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
