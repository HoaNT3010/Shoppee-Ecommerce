using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeeEcommerce.Application.UseCases.Orders.ListUserOrders;
using ShoppeeEcommerce.SharedViewModels.Models.Common;
using ShoppeeEcommerce.SharedViewModels.Models.Orders.ListUserOrders;
using ShoppeeEcommerce.WebAPI.Configuration.Services;
using ShoppeeEcommerce.WebAPI.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeeEcommerce.WebAPI.Endpoints.Orders.ListUserOrders
{
    public class ListUserOrdersEndpoint
        : BaseEndpoint<ListUserOrdersRequest, PagedList<UserOrderSummary>>
    {
        public ListUserOrdersEndpoint(ISender sender) : base(sender)
        {
        }

        [HttpGet("api/v{version:apiVersion}/user/orders")]
        [ApiVersion(1)]
        [Authorize(Policy = AuthorizationPolicies.AuthenticatedUser)]
        [SwaggerOperation(
            Summary = "List all user's order with filtering, sorting and paging. For AUTHENTICATED users only.",
            Tags = new[] { EndpointTags.Orders })]
        public override async Task<ActionResult<PagedList<UserOrderSummary>>> HandleAsync(
            [FromQuery] ListUserOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            var userId = HttpContext.User.GetUserId();
            var query = new ListUserOrdersQuery(userId,
                request.Status,
                request.SortBy,
                request.SortDesc,
                // Default page index 1
                request.PageIndex ?? 1,
                // Default page size 10
                request.PageSize ?? 10);
            var result = await sender.Send(query, cancellationToken);
            return result.ToActionResult();
        }
    }
}
