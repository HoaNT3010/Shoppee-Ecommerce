using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Orders.AdminViewDetail;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.AdminDetail;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Orders.AdminViewDetail
{
    public class AdminViewOrderDetailEndpoint
        : BaseEndpoint<PathGuidIdRequest, AdminViewOrderDetailResponse>
    {
        public AdminViewOrderDetailEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/admin/orders/{id}")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        [SwaggerOperation(
            Summary = "Get an order with full detail. For ADMIN users only.",
            Tags = new[] { EndpointTags.OrdersAdmin })]
        public override async Task<ActionResult<AdminViewOrderDetailResponse>> HandleAsync(
            [FromRoute] PathGuidIdRequest request, CancellationToken cancellationToken = default)
        {
            var query = new AdminViewOrderDetailQuery(Guid.Parse(request.Id));
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
